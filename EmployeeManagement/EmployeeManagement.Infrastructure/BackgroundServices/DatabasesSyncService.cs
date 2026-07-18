using Internship.EmployeeManagement.Core.Entities;
using Internship.EmployeeManagement.Core.Interfaces;
using Internship.EmployeeManagement.Core.Interfaces.Employee;
using Internship.EmployeeManagement.Infrastructure.Presistence.Data;
using Internship.EmployeeManagement.Infrastructure.Presistence.Repositories.Employee;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Serilog;

namespace Internship.EmployeeManagement.Infrastructure.BackgroundServices
{
    public class DatabasesSyncService(AppSqlServerDbContext appSqlServerDbContext, AppMongoDbContext appMongoDbContext, MongoEmployeeHistory mongoEmployeeHistory, SqlServerEmployeeHistory sqlServerEmployeeHistory)
    {
        private readonly AppSqlServerDbContext _appSqlServerDbContext = appSqlServerDbContext;
        private readonly AppMongoDbContext _appMongoDbContext = appMongoDbContext;
        private readonly MongoEmployeeHistory _mongoEmployeeHistory = mongoEmployeeHistory;
        private readonly SqlServerEmployeeHistory _sqlServerEmployeeHistory = sqlServerEmployeeHistory;
        private DateTime _sqlToMongoSyncStartTime;

        public async Task Sync()
        {
            Log.Information("Sync process has started");

            var lastSyncDateTime = ReadLastSyncDateTime();

            var processedIdsInfo = await ExecuteOneWaySync(
                                    sourceContext: _appSqlServerDbContext, 
                                    destinationContext: _appMongoDbContext, 
                                    sourceHistoryRepository: _sqlServerEmployeeHistory, 
                                    destinationHistoryRepository: _mongoEmployeeHistory,
                                    lastSyncDateTime: lastSyncDateTime);

            await ExecuteOneWaySync(
                sourceContext: _appMongoDbContext,
                destinationContext: _appSqlServerDbContext,
                sourceHistoryRepository: _mongoEmployeeHistory,
                destinationHistoryRepository: _sqlServerEmployeeHistory,
                lastSyncDateTime: lastSyncDateTime,
                excludedIdsInfo: processedIdsInfo);

            SaveLastSyncDateTime();

            Log.Information("Sync process has been completed successfully");

        }


        private static async Task<(List<Guid> processedIds, DateTime saveDateTime)> ExecuteOneWaySync<TSource, TDestination>(
            TSource sourceContext, 
            TDestination destinationContext, 
            IEmployeeHistory sourceHistoryRepository, 
            IEmployeeHistory destinationHistoryRepository, 
            DateTime lastSyncDateTime,
            (List<Guid> ids, DateTime saveDateTime)? excludedIdsInfo = null)

            where TSource : DbContext, ISwappableDbContext
            where TDestination : DbContext, ISwappableDbContext
        {
            Log.Information($"starting unidirectional sync process from {typeof(TSource).Name} to {typeof(TDestination).Name}");

            int recordsToBeLoadedCount = 10;

            List<EmployeeHistoryEntity?> sourceHistoryRecords;

            List<Guid> processedIds = new(), batchProcessedIds;

            var processedIdsDictionary = excludedIdsInfo?.ids.ToDictionary(x => x, x => excludedIdsInfo.Value.saveDateTime);

            int i = 0;

            do
            {
                sourceHistoryRecords = await sourceHistoryRepository.GetEmployeesHistoryRecentRecordsInBatches(lastSyncDateTime, recordsToBeLoadedCount, i);

                Log.Information($"{sourceHistoryRecords.Count} source history records of batch index {i} have been retrieved successfully");

                batchProcessedIds = await MigrateChanges(sourceHistoryRecords, sourceContext, destinationContext, destinationHistoryRepository.GetLastEmployeeHistoryRecord, processedIdsDictionary);

                Log.Information($"{batchProcessedIds.Count} records have been synced successfully");

                processedIds.AddRange(batchProcessedIds);

                i++;
            }
            while (sourceHistoryRecords.Any());

            await destinationContext.SaveChangesAsync();

            Log.Information($"unidirectional sync process from {typeof(TSource).Name} to {typeof(TDestination).Name} has been completed successfully");

            return (processedIds, DateTime.UtcNow);
        }


        private static async Task<List<Guid>> MigrateChanges<TSource, TDestination>(List<EmployeeHistoryEntity?> sourceHistoryRecords, 
            TSource sourceContext, 
            TDestination destinationContext, 
            Func<Guid, Task<EmployeeHistoryEntity?>> getHistoryRecordFromDestinationFunc,
            Dictionary<Guid, DateTime>? processedIdsDictionary)
            
            where TSource: DbContext, ISwappableDbContext
            where TDestination: DbContext, ISwappableDbContext
        {

            EmployeeHistoryEntity? destinationHistoryRecord;

            Guid? processedId;

            List<Guid> processedIds = new();

            var filteredHistoryRecords = ExcludeProcessedIds(processedIdsDictionary, sourceHistoryRecords);

            Log.Information($"{sourceHistoryRecords.Count - filteredHistoryRecords.Count} records have been excluded from sync process");

            foreach (var sourceHistoryRecord in filteredHistoryRecords)
            {

                destinationHistoryRecord = await getHistoryRecordFromDestinationFunc.Invoke(sourceHistoryRecord.EmployeeId);

                processedId = await ProcessEntry(destinationHistoryRecord, sourceContext, destinationContext, sourceHistoryRecord);

                if(processedId != null)
                {
                    processedIds.Add(processedId.Value);
                }
            }

            return processedIds;
        }

        private static List<EmployeeHistoryEntity?> ExcludeProcessedIds(Dictionary<Guid, DateTime>? processedIdsDictionary, List<EmployeeHistoryEntity?> historyRecords)
        {
            if (processedIdsDictionary is null)
                return historyRecords;

            List<EmployeeHistoryEntity?> filteredRecords = new();

            foreach (var record in historyRecords)
            {
                if (processedIdsDictionary.ContainsKey(record?.EmployeeId ?? Guid.Empty))
                {
                    var isEntityDeleted = (record.EndDateTime > processedIdsDictionary[record.EmployeeId]) && record.EndDateTime < DateTime.MaxValue;

                    var isEntityUpdated = (record.CreationDateTime > processedIdsDictionary[record.EmployeeId]);

                    if (isEntityDeleted || isEntityUpdated)
                    {
                        filteredRecords.Add(record);
                    }

                }
                else
                {
                    filteredRecords.Add(record);
                }
            }

            return filteredRecords;
        }


        private static async Task<Guid?> ProcessEntry<TSource, TDestination>(EmployeeHistoryEntity? destinationHistoryRecord, TSource sourceContext, TDestination destinationContext, EmployeeHistoryEntity sourceHistoryRecord)
            where TSource : DbContext, ISwappableDbContext
            where TDestination : DbContext, ISwappableDbContext
        {
            bool doesEntityHaveHistoryInDest = destinationHistoryRecord is not null;

            if(doesEntityHaveHistoryInDest)
            {
                var isEntityDeletedFromDest = destinationHistoryRecord?.EndDateTime != DateTime.MaxValue;

                if (isEntityDeletedFromDest)
                    return null;

                var isEntityDeletedFromSrc = sourceHistoryRecord?.EndDateTime != DateTime.MaxValue;

                if (isEntityDeletedFromSrc)
                {
                    await DeleteFromDestination(sourceHistoryRecord.EmployeeId, destinationContext);
                    return sourceHistoryRecord.EmployeeId;
                }

                var isEntityUpdated = sourceHistoryRecord.CreationDateTime > destinationHistoryRecord?.CreationDateTime;

                if (isEntityUpdated)
                {
                    await UpdateDestination(sourceHistoryRecord.EmployeeId, sourceContext, destinationContext);
                    return sourceHistoryRecord.EmployeeId;
                }

            }
            else
            {
                var doesEntityExistsInSrc = sourceHistoryRecord?.EndDateTime == DateTime.MaxValue;

                if (doesEntityExistsInSrc)
                {
                    await AddToDestination(sourceHistoryRecord.EmployeeId, sourceContext, destinationContext);
                    return sourceHistoryRecord.EmployeeId;
                }
            }

            return null;
        }

        private static async Task AddToDestination<TSource, TDestination>(Guid employeeId, TSource sourceDb, TDestination destinationDb)
            where TSource : DbContext, ISwappableDbContext
            where TDestination : DbContext, ISwappableDbContext
        {

            var sourceEntity = await sourceDb.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == employeeId);

            if (sourceEntity is not null)
            {
                await destinationDb.Employees.AddAsync(sourceEntity);
                Log.Information($"entity with id {employeeId} has been arranged for addition");
            }
        }


        private static async Task DeleteFromDestination<TDestination>(Guid employeeId, TDestination destinationDb)
            where TDestination : DbContext, ISwappableDbContext
        {

            var destinationEntity = await destinationDb.Employees.FindAsync(employeeId);

            if (destinationEntity is not null)
            {
                destinationDb.Remove(destinationEntity);
                Log.Information($"entity with id {employeeId} has been arranged for deletion");
            }
        }

        private static async Task UpdateDestination<TSource, TDestination>(Guid employeeId, TSource sourceContext, TDestination destinationContext)

            where TSource : DbContext, ISwappableDbContext
            where TDestination : DbContext, ISwappableDbContext
        {

            var sourceEntity = await sourceContext.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == employeeId);

            var destinationEntity = await destinationContext.Employees.FindAsync(employeeId);

            CopyData(sourceEntity, destinationEntity);

            Log.Information($"entity with id {employeeId} has been arranged for update");
        }

        private static void CopyData(EmployeeEntity sourceEntity, EmployeeEntity destinationEntity)
        {
            destinationEntity.Age = sourceEntity.Age;
            destinationEntity.Title = sourceEntity.Title;
            destinationEntity.Name = sourceEntity.Name;
        }



        private static void SaveLastSyncDateTime()
        {
            Log.Information("Saving last sync dateTime");

            string filePath = "SyncData.txt";

            File.WriteAllText(filePath, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fffffff"));
        }

        private static DateTime ReadLastSyncDateTime()
        {
            Log.Information("Reading last sync dateTime");

            string fileContent = "";

            if (File.Exists("SyncData.txt"))
            {
                fileContent = File.ReadAllText("SyncData.txt");
            }

            if (!DateTime.TryParse(fileContent, out DateTime savedDateTime))
            {
                savedDateTime = DateTime.MinValue;
            }

            Log.Information($"last sync dateTime is {savedDateTime}");

            return DateTime.SpecifyKind(savedDateTime, DateTimeKind.Utc);
        }
    }
}

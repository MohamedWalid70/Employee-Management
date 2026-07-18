using Internship.EmployeeManagement.Core.Entities;
using Internship.EmployeeManagement.Infrastructure.Presistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Internship.EmployeeManagement.Infrastructure.Presistence.Interceptors
{
    public class EmployeeHistoryInterceptor: SaveChangesInterceptor
    {
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, 
            InterceptionResult<int> result, 
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context is null || context is not AppMongoDbContext)
                return await base.SavingChangesAsync(eventData, result, cancellationToken);


            var entries = context.ChangeTracker.Entries<EmployeeEntity>()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
                    .ToList();

            foreach (var entry in entries)
            {
                await ProcessEntry(entry, context);
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static async Task ProcessEntry(EntityEntry<EmployeeEntity> entry, DbContext context)
        {

            var propertiesDictionary = entry.Properties.ToDictionary(prop => prop.Metadata.Name, prop => prop.CurrentValue);


            if (entry.State == EntityState.Modified)
            {
                var employeeHistoryEntity = new EmployeeHistoryEntity
                {
                    OperationType = entry.State.ToString(),
                    EmployeeId = entry.Entity.Id
                };

                CopyEmployeeCurrentPropertiesValues(employeeHistoryEntity, propertiesDictionary);

                employeeHistoryEntity.CreationDateTime = DateTime.UtcNow;
                employeeHistoryEntity.EndDateTime = DateTime.MaxValue;

                var lastHistoryRecord = await context.Set<EmployeeHistoryEntity>().Where(eh => eh.EmployeeId == entry.Entity.Id).OrderBy(eh => eh.CreationDateTime).LastOrDefaultAsync();

                lastHistoryRecord?.EndDateTime = DateTime.UtcNow;

                await context.Set<EmployeeHistoryEntity>().AddAsync(employeeHistoryEntity);
            }
            else if (entry.State == EntityState.Added)
            {
                var employeeHistoryEntity = new EmployeeHistoryEntity
                {
                    OperationType = entry.State.ToString(),
                    EmployeeId = entry.Entity.Id
                };

                CopyEmployeeCurrentPropertiesValues(employeeHistoryEntity, propertiesDictionary);

                employeeHistoryEntity.CreationDateTime = DateTime.UtcNow;
                employeeHistoryEntity.EndDateTime = DateTime.MaxValue;

                await context.Set<EmployeeHistoryEntity>().AddAsync(employeeHistoryEntity);
            }
            else if (entry.State == EntityState.Deleted)
            {
                var lastHistoryRecord = await context.Set<EmployeeHistoryEntity>().Where(eh => eh.EmployeeId == entry.Entity.Id).OrderBy(eh => eh.CreationDateTime).LastOrDefaultAsync();

                lastHistoryRecord?.EndDateTime = DateTime.UtcNow;
            }
        }

        private static void CopyEmployeeCurrentPropertiesValues(EmployeeHistoryEntity employeeHistoryEntity, Dictionary<string, Object?> propertiesDictionary)
        {
            employeeHistoryEntity.Age = (byte)propertiesDictionary[nameof(EmployeeEntity.Age)];
            employeeHistoryEntity.Name = (string)propertiesDictionary[nameof(EmployeeEntity.Name)];
            employeeHistoryEntity.Title = (string)propertiesDictionary[nameof(EmployeeEntity.Title)];
        }
    }
}

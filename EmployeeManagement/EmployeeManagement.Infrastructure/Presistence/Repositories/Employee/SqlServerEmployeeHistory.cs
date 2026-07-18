using Internship.EmployeeManagement.Core.Entities;
using Internship.EmployeeManagement.Core.Interfaces.Employee;
using Internship.EmployeeManagement.Infrastructure.Presistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Internship.EmployeeManagement.Infrastructure.Presistence.Repositories.Employee
{
    public class SqlServerEmployeeHistory(AppSqlServerDbContext appSqlServerDbContext) : IEmployeeHistory
    {
        private readonly AppSqlServerDbContext _appDbContext = appSqlServerDbContext;

        public async Task<IEnumerable<EmployeeHistoryEntity>> GetEmployeeHistoryByEmployeeIdAsync(Guid employeeId)
        {
            var records = await _appDbContext.Employees
                     .TemporalAll()
                     .Where(employee => employee.Id == employeeId)
                     .OrderBy(employee => EF.Property<DateTime>(employee, "PeriodStart"))
                     .Select(employee => new EmployeeHistoryEntity
                     {
                         Age = employee.Age,
                         CreationDateTime = EF.Property<DateTime>(employee, "PeriodStart"),
                         EndDateTime = EF.Property<DateTime>(employee, "PeriodEnd"),
                         Name = employee.Name,
                         Title = employee.Title

                     })
                     .ToListAsync();

            ExtractEmployeeRecordOperationData(records);

            return records;
        }

        private static void ExtractEmployeeRecordOperationData(List<EmployeeHistoryEntity> records)
        {
            for (int i = 0; i < records.Count; i++)
            {

                if (records.Count > 1)
                {
                    if (i == 0)
                        records[i].OperationType = "Creation";
                    else if (i == records.Count - 1 && records[i].EndDateTime != DateTime.MaxValue)
                        records[i].OperationType = "Deletion";
                    else
                        records[i].OperationType = "Update";
                }
                else
                {
                    records[i].OperationType = records[i].EndDateTime != DateTime.MaxValue ? "Deletion" : "Creation";
                }
            }
        }

        public async Task<EmployeeHistoryEntity?> GetLastEmployeeHistoryRecord(Guid employeeId)
        {
            var record = await _appDbContext.Employees
                     .TemporalAll()
                     .Where(employee => employee.Id == employeeId)
                     .Select(employee => new EmployeeHistoryEntity
                     {
                         Age = employee.Age,
                         CreationDateTime = EF.Property<DateTime>(employee, "PeriodStart"),
                         EndDateTime = EF.Property<DateTime>(employee, "PeriodEnd"),
                         Name = employee.Name,
                         Title = employee.Title

                     })
                     .OrderBy(eh => eh.CreationDateTime)
                     .LastOrDefaultAsync();

            return record;
        }

        public async Task<List<EmployeeHistoryEntity?>> GetEmployeesHistoryRecentRecordsInBatches(DateTime startEndpointDateTime, int batchSize, int iteration)
        {
            var employeeHistoryLastRecords = await _appDbContext.Employees.TemporalAll()
                                            .Where(e => EF.Property<DateTime>(e, "PeriodStart") > startEndpointDateTime || (EF.Property<DateTime>(e, "PeriodEnd") > startEndpointDateTime && EF.Property<DateTime>(e, "PeriodEnd") != DateTime.MaxValue))
                                            .Select(employee => new EmployeeHistoryEntity
                                            {
                                                EmployeeId = employee.Id,
                                                Age = employee.Age,
                                                CreationDateTime = EF.Property<DateTime>(employee, "PeriodStart"),
                                                EndDateTime = EF.Property<DateTime>(employee, "PeriodEnd"),
                                                Name = employee.Name,
                                                Title = employee.Title
                                            })
                                            .GroupBy(e => e.EmployeeId)
                                            .Select(g => g.OrderByDescending(e => e.CreationDateTime).FirstOrDefault())
                                            .Skip(iteration * batchSize).Take(batchSize).ToListAsync();

            return employeeHistoryLastRecords;
        }
    }
}

using Internship.EmployeeManagement.Core.Entities;
using Internship.EmployeeManagement.Core.Interfaces.Employee;
using Internship.EmployeeManagement.Infrastructure.Presistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Internship.EmployeeManagement.Infrastructure.Presistence.Repositories.Employee
{
    public class MongoEmployeeHistory(AppMongoDbContext appMongoDbContext) : IEmployeeHistory
    {
        private readonly AppMongoDbContext _appDbContext = appMongoDbContext;

        public async Task<IEnumerable<EmployeeHistoryEntity>> GetEmployeeHistoryByEmployeeIdAsync(Guid employeeId)
        {

            var records = await _appDbContext.EmployeesHistory
                    .Where(eh => eh.EmployeeId == employeeId)
                    .OrderBy(eh => eh.CreationDateTime)
                    .ToListAsync();

            return records;
        }

        public async Task<List<EmployeeHistoryEntity?>> GetEmployeesHistoryRecentRecordsInBatches(DateTime startEndpointDateTime, int batchSize, int iteration)
        {
            var employeesHistoryRecordsTmp = await _appDbContext.EmployeesHistory
                                            .Where(eh => eh.CreationDateTime > startEndpointDateTime || (eh.EndDateTime > startEndpointDateTime && eh.EndDateTime != DateTime.MaxValue))
                                            .Skip(iteration * batchSize).Take(batchSize).ToListAsync();

            var employeesHistoryRecords = employeesHistoryRecordsTmp.GroupBy(eh => eh.EmployeeId)
                                           .Select(ehGroup => ehGroup.OrderBy(eh => eh.CreationDateTime).LastOrDefault()).ToList();

            return employeesHistoryRecords;
        }

        public async Task<EmployeeHistoryEntity?> GetLastEmployeeHistoryRecord(Guid employeeId)
        {
            var record = await _appDbContext.EmployeesHistory
                    .Where(eh => eh.EmployeeId == employeeId)
                    .OrderBy(eh => eh.CreationDateTime)
                    .LastOrDefaultAsync();

            return record;
        }
    }
}

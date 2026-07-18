using Internship.EmployeeManagement.Core.Entities;

namespace Internship.EmployeeManagement.Core.Interfaces.Employee
{
    public interface IEmployeeHistory
    {
        Task<IEnumerable<EmployeeHistoryEntity>> GetEmployeeHistoryByEmployeeIdAsync(Guid employeeId);

        Task<EmployeeHistoryEntity?> GetLastEmployeeHistoryRecord(Guid employeeId);

        Task<List<EmployeeHistoryEntity?>> GetEmployeesHistoryRecentRecordsInBatches(DateTime startEndpointDateTime, int batchSize, int iteration);
    }
}

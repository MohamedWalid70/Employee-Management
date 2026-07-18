using Internship.EmployeeManagement.Core.Entities;

namespace Internship.EmployeeManagement.Core.Interfaces.Employee
{
    public interface IEmployeeRepository<TContext>
    {
        Task AddEmployeeAsync(EmployeeEntity employee);
        void RemoveEmployee(EmployeeEntity employee);
        IAsyncEnumerable<EmployeeEntity> GetPaginatedEmployeesAsync(int pageNumber, int pageSize);
        Task<EmployeeEntity?> GetEmployeeByIdAsync(Guid employeeId);
        Task<bool> DoesEmployeeExist(Guid employeeId);
        Task RemoveEmployeeByIdAsync(Guid employeeId);
    }
}

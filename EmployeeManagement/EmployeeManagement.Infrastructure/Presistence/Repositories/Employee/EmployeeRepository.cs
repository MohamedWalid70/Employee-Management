using Internship.EmployeeManagement.Core.Entities;
using Internship.EmployeeManagement.Core.Interfaces;
using Internship.EmployeeManagement.Core.Interfaces.Employee;
using Microsoft.EntityFrameworkCore;

namespace Internship.EmployeeManagement.Infrastructure.Presistence.Repositories.Employee
{
    public class EmployeeRepository<TContext>(TContext appDbContext) : IEmployeeRepository<TContext> where TContext : ISwappableDbContext
    {
        TContext _appDbContext = appDbContext;

        public async Task AddEmployeeAsync(EmployeeEntity employee)
        {
            await _appDbContext.Employees.AddAsync(employee);
        }

        public async Task<EmployeeEntity?> GetEmployeeByIdAsync(Guid employeeId)
        {
            return await _appDbContext.Employees.FindAsync(employeeId);
        }



        public IAsyncEnumerable<EmployeeEntity> GetPaginatedEmployeesAsync(int pageNumber, int pageSize)
        {
            return _appDbContext.Employees
                .AsNoTracking()
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .AsAsyncEnumerable();
        }

        public void RemoveEmployee(EmployeeEntity employee)
        {
            _appDbContext.Employees.Remove(employee); 
        }

        public async Task<bool> DoesEmployeeExist(Guid employeeId)
        {
            return await _appDbContext.Employees.AnyAsync(emp => emp.Id == employeeId);
        }

        public async Task RemoveEmployeeByIdAsync(Guid employeeId)
        {
            await _appDbContext.Employees.Where(e => e.Id == employeeId).ExecuteDeleteAsync();
        }
    }
}

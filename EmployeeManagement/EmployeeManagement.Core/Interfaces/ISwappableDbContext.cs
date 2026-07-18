using Internship.EmployeeManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Internship.EmployeeManagement.Core.Interfaces
{
    public interface ISwappableDbContext
    {
        public DbSet<EmployeeEntity> Employees { get; set; }
        public Task SaveChangesAsync();
    }
}

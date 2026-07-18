using Internship.EmployeeManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Internship.EmployeeManagement.Core.Interfaces
{
    public interface IWriteDbContext : ISwappableDbContext
    {
        public DbSet<EmployeeHistoryEntity> EmployeesHistory { get; set; }

    }
}

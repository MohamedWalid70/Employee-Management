using Internship.EmployeeManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Internship.EmployeeManagement.Core.Interfaces
{
    public interface IReadDbContext: ISwappableDbContext
    {
        public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }

    }
}

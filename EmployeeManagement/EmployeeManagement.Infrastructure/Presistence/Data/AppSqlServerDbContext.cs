using Internship.EmployeeManagement.Core.Entities;
using Internship.EmployeeManagement.Core.Interfaces;
using Internship.EmployeeManagement.Infrastructure.Presistence.SqlServerConfigurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Internship.EmployeeManagement.Infrastructure.Presistence.Data
{
    public class AppSqlServerDbContext : IdentityDbContext<UserEntity, IdentityRole<int>, int>, IReadDbContext
    {
        public DbSet<EmployeeEntity> Employees { get; set; }
        public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }

        public AppSqlServerDbContext(DbContextOptions<AppSqlServerDbContext> dbContextOptions): base(dbContextOptions)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public async Task SaveChangesAsync()
        {
            var res = await base.SaveChangesAsync();
        }
    }
}

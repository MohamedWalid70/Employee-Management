using Internship.EmployeeManagement.Core.Entities;
using Internship.EmployeeManagement.Core.Interfaces;
using Internship.EmployeeManagement.Infrastructure.Presistence.Interceptors;
using Internship.EmployeeManagement.Infrastructure.Presistence.MongoConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Internship.EmployeeManagement.Infrastructure.Presistence.Data
{
    public class AppMongoDbContext(DbContextOptions<AppMongoDbContext> dbContextOptions) : DbContext(dbContextOptions), IWriteDbContext
    {
        public DbSet<EmployeeEntity> Employees { get; set; }
        public DbSet<EmployeeHistoryEntity> EmployeesHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeHistoryConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(new EmployeeHistoryInterceptor());
            base.OnConfiguring(optionsBuilder);
        }

        public async Task SaveChangesAsync()
        {
            await base.SaveChangesAsync();
        }
    }
}

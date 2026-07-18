using Internship.EmployeeManagement.Infrastructure.BackgroundServices;
using Internship.EmployeeManagement.Infrastructure.Presistence.Data;
using Internship.EmployeeManagement.Infrastructure.Presistence.Repositories.Employee;
using Quartz;

namespace Internship.EmployeeManagement.Infrastructure.QuartzJobs
{
    public class DatabasesSyncJob(AppSqlServerDbContext appSqlServerDbContext, AppMongoDbContext appMongoDbContext, MongoEmployeeHistory mongoEmployeeHistory, SqlServerEmployeeHistory sqlServerEmployeeHistory) : IJob
    {
        private readonly DatabasesSyncService _databasesSyncService = new(appSqlServerDbContext, appMongoDbContext, mongoEmployeeHistory, sqlServerEmployeeHistory);
        public async Task Execute(IJobExecutionContext context)
        {
            await _databasesSyncService.Sync();
        }
    }
}

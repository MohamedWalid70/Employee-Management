using Internship.EmployeeManagement.Application.CustomTypes;
using Internship.EmployeeManagement.Application.Features.Employees.Commands.CreateEmployee;
using Internship.EmployeeManagement.Core.Entities;
using Internship.EmployeeManagement.Core.Interfaces;
using Internship.EmployeeManagement.Core.Interfaces.Employee;
using Internship.EmployeeManagement.Infrastructure.MessageBroker;
using Internship.EmployeeManagement.Infrastructure.Presistence.Data;
using Internship.EmployeeManagement.Infrastructure.Presistence.Repositories;
using Internship.EmployeeManagement.Infrastructure.Presistence.Repositories.Employee;
using Internship.EmployeeManagement.Infrastructure.QuartzJobs;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Quartz;
using Serilog;

namespace Internship.EmployeeManagement.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static void RegisterInfrastructureServices(this WebApplicationBuilder builder)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt",
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

            builder.Services.AddDbContextPool<AppSqlServerDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

            builder.Services.AddDbContext<AppMongoDbContext>(options => options.UseMongoDB(builder.Configuration.GetConnectionString("MongoDefault")));

            builder.Services.AddScoped<IReadDbContext, AppSqlServerDbContext>();

            builder.Services.AddScoped<IWriteDbContext, AppMongoDbContext>();

            builder.Services.AddKeyedScoped<IEmployeeHistory, SqlServerEmployeeHistory>(ContextType.Read);
            builder.Services.AddKeyedScoped<IEmployeeHistory, MongoEmployeeHistory>(ContextType.Write);


            builder.Services.AddScoped(typeof(IEmployeeRepository<>), typeof(EmployeeRepository<>));


            builder.Services.AddScoped<MongoEmployeeHistory>();
            builder.Services.AddScoped<SqlServerEmployeeHistory>();

            builder.Services.Configure<MessageBrokerSettings>(builder.Configuration.GetSection("MessageBroker"));


            builder.Services.AddMassTransit(busConfig =>
            {
                busConfig.SetKebabCaseEndpointNameFormatter();

                busConfig.AddConsumers(typeof(EmployeeCreatedEventConsumer).Assembly);

                busConfig.UsingRabbitMq((context, config) =>
                {
                    var messageBrokerSettings = context.GetRequiredService<IOptions<MessageBrokerSettings>>();

                    config.Host(messageBrokerSettings.Value.Host);

                    config.ConfigureEndpoints(context);

                });
            });

            builder.Services.AddScoped<IEventBus, EventBus>();


            builder.Services.AddQuartz(config =>
            {
                var jobKey = JobKey.Create("DatabaseSync");

                config.AddJob<DatabasesSyncJob>(ops => ops.WithIdentity(jobKey).DisallowConcurrentExecution())
                    .AddTrigger(trigger => trigger.ForJob(jobKey).WithSchedule(CronScheduleBuilder.CronSchedule("0 0/15 * * * ?")));

            });

            builder.Services.AddQuartzHostedService();


            builder.Services.AddIdentity<UserEntity, IdentityRole<int>>(options => {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = true;
                })
                .AddRoles<IdentityRole<int>>()
                .AddEntityFrameworkStores<AppSqlServerDbContext>();

            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        }
    }
}

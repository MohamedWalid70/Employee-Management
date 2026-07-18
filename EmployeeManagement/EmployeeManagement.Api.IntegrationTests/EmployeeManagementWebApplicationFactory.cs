using Internship.EmployeeManagement.Api.IntegrationTests.ExternalServices;
using Internship.EmployeeManagement.Api.IntegrationTests.Models;
using Internship.EmployeeManagement.Api.IntegrationTests.NSwag.Generated;
using Internship.EmployeeManagement.Core.Interfaces;
using Internship.EmployeeManagement.Infrastructure.Presistence.Data;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace Internship.EmployeeManagement.Api.IntegrationTests
{
    internal sealed class EmployeeManagementWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private ApiSettings? _settings;
        private HttpClient? _httpClient;

        public AuthenticationHeaderValue AuthorizationHeader { 
            set 
            { 
                _httpClient?.DefaultRequestHeaders.Authorization = value; 
            } 
        }
        protected sealed override void ConfigureWebHost(IWebHostBuilder builder)
        {

            builder.ConfigureAppConfiguration(config =>
            {

                var testConfig = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

                _settings = new ApiSettings();
                testConfig.GetSection("ApiSettings").Bind(_settings);

                builder.UseTestServer();
            });

            builder.ConfigureServices(services => {

                var descriptor = services.SingleOrDefault(
                                d => d.ServiceType == typeof(IEventBus));

                if (descriptor != null) 
                    services.Remove(descriptor);

                services.AddSingleton<IEventBus, EventBus>();

            });
        }

        public EmployeeManagementClient CreateEmployeeClient()
        {
            _httpClient = CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            if (_settings?.BaseUrl is not null)
                return new EmployeeManagementClient(_settings.BaseUrl, _httpClient);

            throw new ArgumentNullException();
        }

        public async Task InitializeAsync()
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppSqlServerDbContext>();

            if (_settings is { DatabaseAutoDeletion: true })
                await db.Database.EnsureDeletedAsync();
            
            await db.Database.MigrateAsync();

        }

        public new async Task DisposeAsync()
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppSqlServerDbContext>();

            if (_settings is { DatabaseAutoDeletion: true })
                await db.Database.EnsureDeletedAsync();
        }
    }
}

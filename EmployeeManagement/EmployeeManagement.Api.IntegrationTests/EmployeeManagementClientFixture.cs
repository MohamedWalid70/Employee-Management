using Internship.EmployeeManagement.Api.IntegrationTests.ExternalServices;
using Internship.EmployeeManagement.Api.IntegrationTests.NSwag.Generated;
using Internship.EmployeeManagement.Core.Interfaces;
using MassTransit.Testing;

namespace Internship.EmployeeManagement.Api.IntegrationTests
{
    public class EmployeeManagementClientFixture
    {
        private EmployeeManagementWebApplicationFactory _employeeManagementWebApplicationFactory;
        public EmployeeManagementClient EmployeeManagementClient { get; }
        public EventBus? EventBus { get
            {
                field ??= _employeeManagementWebApplicationFactory.Services.GetService(typeof(IEventBus)) as EventBus;
                return field;
            }
        }
        public string AuthorizationToken { 
            set 
            {
                _employeeManagementWebApplicationFactory.AuthorizationHeader = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", value);
            }
        }
        public EmployeeManagementClientFixture()
        {

            _employeeManagementWebApplicationFactory ??= new EmployeeManagementWebApplicationFactory();
            EmployeeManagementClient ??= _employeeManagementWebApplicationFactory.CreateEmployeeClient();
        }

    }
}

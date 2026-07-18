using AutoFixture;
using Bogus;
using Internship.EmployeeManagement.Api.IntegrationTests.ExternalServices;
using Internship.EmployeeManagement.Api.IntegrationTests.FixtureCustomizations;
using Internship.EmployeeManagement.Api.IntegrationTests.NSwag.Generated;
using Internship.EmployeeManagement.Api.IntegrationTests.TestingHelpers;
using Internship.EmployeeManagement.Application.Features.Employees.Commands.CreateEmployee;
using Internship.EmployeeManagement.Application.Features.Employees.Commands.DeleteEmployee;
using Internship.EmployeeManagement.Application.Features.Employees.Commands.UpdateEmployee;


namespace Internship.EmployeeManagement.Api.IntegrationTests.Employees
{
    public partial class EmployeeManagementApiTests: IAssemblyFixture<EmployeeManagementClientFixture>
    {
        private readonly EmployeeManagementClient _client;
        private readonly Fixture _fixture;
        private readonly EmployeeManagementClientFixture _employeeManagementClientFixture;
        private CreateUserCommandParam? _seededUser;
        private readonly EventBus? _bus;
        public EmployeeManagementApiTests(EmployeeManagementClientFixture clientFixture)
        {
            _fixture = new();
            _client = clientFixture.EmployeeManagementClient;
            _employeeManagementClientFixture = clientFixture;

            _bus = _employeeManagementClientFixture.EventBus;

            _bus?.Subscribe<EmployeeCreatedEvent, EmployeeCreatedEventConsumer>();
            _bus?.Subscribe<EmployeeDeleteEvent, EmployeeDeletedEventConsumer>();
            _bus?.Subscribe<EmployeeUpdatedEvent, EmployeeUpdatedEventConsumer>();

            _fixture.Customize(new UserCustomization(new Faker()));
        }
        
        private async Task CreateAndAuthenticateRandomUser()
        {
            _seededUser = _fixture.Create<CreateUserCommandParam>();

            await _client.UsersPOSTAsync(_seededUser);

            var loginQueryParam = _fixture.Build<LoginQueryParam>()
                .With(x => x.Email, _seededUser.Email)
                .With(x => x.Password, _seededUser.Password)
                .Create();

            await HelperMethods.AuthenticateUser(loginQueryParam, _client, _fixture, _employeeManagementClientFixture);
        }


    }
}

using AutoFixture;
using Bogus;
using Internship.EmployeeManagement.Api.IntegrationTests.FixtureCustomizations;
using Internship.EmployeeManagement.Api.IntegrationTests.NSwag.Generated;

namespace Internship.EmployeeManagement.Api.IntegrationTests.Auth
{
    public partial class AuthTests : IAssemblyFixture<EmployeeManagementClientFixture>, IAsyncLifetime
    {
        private readonly EmployeeManagementClient _client;
        private readonly IFixture _fixture;
        private CreateUserCommandParam? _seededUser;
        private EmployeeManagementClientFixture _employeeManagementClientFixture;

        public AuthTests(EmployeeManagementClientFixture clientFixture)
        {
            _fixture = new Fixture();
            _client = clientFixture.EmployeeManagementClient;

            _employeeManagementClientFixture = clientFixture;

            _fixture.Customize(new UserCustomization(new Faker()));
        }

        public async Task DisposeAsync()
        {
        }

        public async Task InitializeAsync()
        {
            _seededUser = _fixture.Create<CreateUserCommandParam>();
            await _client.UsersPOSTAsync(_seededUser);
        }

    }

}

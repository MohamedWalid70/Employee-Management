using AutoFixture;
using Bogus;
using Internship.EmployeeManagement.Api.IntegrationTests.FixtureCustomizations;
using Internship.EmployeeManagement.Api.IntegrationTests.NSwag.Generated;

namespace Internship.EmployeeManagement.Api.IntegrationTests.Users
{
    public partial class UserApiTests : IAssemblyFixture<EmployeeManagementClientFixture>
    {
        private readonly EmployeeManagementClient _client;
        private readonly IFixture _fixture;
        private readonly EmployeeManagementClientFixture _employeeManagementClientFixture;

        public UserApiTests(EmployeeManagementClientFixture clientFixture)
        {
            _fixture = new Fixture();
            _client = clientFixture.EmployeeManagementClient;
            _employeeManagementClientFixture = clientFixture;

            _fixture.Customize(new UserCustomization(new Faker()));
        }

    }
}

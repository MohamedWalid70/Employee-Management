using AutoFixture;
using Internship.EmployeeManagement.Api.IntegrationTests.NSwag.Generated;
using Internship.EmployeeManagement.Api.IntegrationTests.TestingHelpers;
using Internship.EmployeeManagement.Application.Features.Employees.Commands.CreateEmployee;
using Microsoft.AspNetCore.Http;
using Shouldly;

namespace Internship.EmployeeManagement.Api.IntegrationTests.Employees
{
    public partial class EmployeeManagementApiTests
    {
        [Fact]
        public async Task CreateEmployee_WithValidInput_ShouldReturn201AndLocationHeader()
        {
            await HelperMethods.AuthenticateAdmin(_client, _fixture, _employeeManagementClientFixture);

            var employee = _fixture.Build<CreateEmployeeCommandParam>().Create();
            var creationResponse = await _client.EmployeesPOSTAsync(employee);

            var isMessageConsumed = _bus?.Consumed.Any(m => m == typeof(EmployeeCreatedEvent));

            var getResponse = await _client.EmployeesGETAsync(creationResponse.Result.Id);

            isMessageConsumed?.ShouldBeTrue();
            creationResponse.StatusCode.ShouldBe(StatusCodes.Status201Created);
            creationResponse.Result.Id.ShouldNotBe(Guid.Empty);
            creationResponse.Headers.ShouldContain(x => x.Key == "Location");

            getResponse.StatusCode.ShouldBe(StatusCodes.Status200OK);
            getResponse.Result.Data.Id.ShouldBe(creationResponse.Result.Id);
            getResponse.Result.Data.Name.ShouldBe(employee.Name);
        }

        [Fact]
        public async Task CreateEmployee_WithUnauthenticatedAccess_ShouldReturn401()
        {
            _employeeManagementClientFixture.AuthorizationToken = "";

            var employee = _fixture.Build<CreateEmployeeCommandParam>().Create();

            var apiCall = async () => await _client.EmployeesPOSTAsync(employee);

            var exception = apiCall.ShouldThrow<EmployeeManagementApiIntegrationTestsException>();

            exception.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
        }


        [Fact]
        public async Task CreateEmployee_WithEmptyBody_ShouldReturn400()
        {
            await HelperMethods.AuthenticateAdmin(_client, _fixture, _employeeManagementClientFixture);

            var apiCall = async () => await _client.EmployeesPOSTAsync(null);

            var exception = await apiCall.ShouldThrowAsync<EmployeeManagementApiIntegrationTestsException>();
            exception.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task CreateEmployee_WithSqlInjectionInName_ShouldReturn200AndGetsStoredSafely()
        {
            await HelperMethods.AuthenticateAdmin(_client, _fixture, _employeeManagementClientFixture);

            var employeeToBeCreated = _fixture.Build<CreateEmployeeCommandParam>().With(x => x.Name, "'; DROP TABLE Employees; --").Create();

            var creationResponse = await _client.EmployeesPOSTAsync(employeeToBeCreated);

            creationResponse.StatusCode.ShouldBe(StatusCodes.Status201Created);
            creationResponse.Result.Id.ShouldNotBe(Guid.Empty);

        }

        [Fact]
        public async Task CreateEmployee_WithUserRoleAndValidInput_ShouldReturn201AndLocationHeader()
        {
            await CreateAndAuthenticateRandomUser();

            var employee = _fixture.Build<CreateEmployeeCommandParam>().Create();
            var creationResponse = await _client.EmployeesPOSTAsync(employee);

            creationResponse.StatusCode.ShouldBe(StatusCodes.Status201Created);

        }
    }
}

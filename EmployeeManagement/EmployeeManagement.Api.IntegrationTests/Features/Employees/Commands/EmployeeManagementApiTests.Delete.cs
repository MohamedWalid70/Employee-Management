using AutoFixture;
using Internship.EmployeeManagement.Api.IntegrationTests.NSwag.Generated;
using Internship.EmployeeManagement.Api.IntegrationTests.TestingHelpers;
using Internship.EmployeeManagement.Application.Features.Employees.Commands.DeleteEmployee;
using Microsoft.AspNetCore.Http;
using Shouldly;

namespace Internship.EmployeeManagement.Api.IntegrationTests.Employees
{
    public partial class EmployeeManagementApiTests
    {
        [Fact]
        public async Task DeleteEmployee_WhenEmployeeExists_ShouldReturn204AndEmployeeIsDeleted()
        {
            await HelperMethods.AuthenticateAdmin(_client, _fixture, _employeeManagementClientFixture);

            var employee = _fixture.Build<CreateEmployeeCommandParam>().Create();

            var createResponse = await _client.EmployeesPOSTAsync(employee);

            var deleteResponse = await _client.EmployeesDELETEAsync(createResponse.Result.Id);

            var isMessageConsumed = _bus?.Consumed.Any(m => m == typeof(EmployeeDeleteEvent));

            var getResponse = await _client.EmployeesGETAsync(createResponse.Result.Id);

            isMessageConsumed?.ShouldBeTrue();
            deleteResponse.StatusCode.ShouldBe(StatusCodes.Status204NoContent);
            getResponse.Result.Data.ShouldBeNull();
        }


        [Fact]
        public async Task DeleteEmployee_WithUnauthenticatedAccess_ShouldReturn401()
        {
            var randomEmployeeId = Guid.NewGuid();

            _employeeManagementClientFixture.AuthorizationToken = "";

            var apiCall = async () => await _client.EmployeesDELETEAsync(randomEmployeeId);

            var exception = apiCall.ShouldThrow<EmployeeManagementApiIntegrationTestsException>();

            exception.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        public async Task DeleteEmployee_WhenEmployeeDoesNotExist_ShouldReturn404()
        {
            var invalidId = Guid.NewGuid();

            await HelperMethods.AuthenticateAdmin(_client, _fixture, _employeeManagementClientFixture);

            var apiCall = async () => await _client.EmployeesDELETEAsync(invalidId);

            var exception = await apiCall.ShouldThrowAsync<EmployeeManagementApiIntegrationTestsException>();

            exception.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        }


        [Fact]
        public async Task DeleteEmployee_WhenCalledTwice_ShouldReturn404()
        {
            await HelperMethods.AuthenticateAdmin(_client, _fixture, _employeeManagementClientFixture);

            var employee = _fixture.Build<CreateEmployeeCommandParam>().Create();

            var createResponse = await _client.EmployeesPOSTAsync(employee);

            var response = await _client.EmployeesDELETEAsync(createResponse.Result.Id);

            response.StatusCode.ShouldBe(StatusCodes.Status204NoContent);

            var apiCall = async () => await _client.EmployeesDELETEAsync(createResponse.Result.Id);

            var exception = await apiCall.ShouldThrowAsync<EmployeeManagementApiIntegrationTestsException>();

            exception.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task DeleteEmployee_WithUserRoleWhenEmployeeExists_ShouldReturn403()
        {
            await CreateAndAuthenticateRandomUser();

            var employee = _fixture.Build<CreateEmployeeCommandParam>().Create();

            var createResponse = await _client.EmployeesPOSTAsync(employee);

            var apiCall = async () => await _client.EmployeesDELETEAsync(createResponse.Result.Id);

            var exception = await apiCall.ShouldThrowAsync<EmployeeManagementApiIntegrationTestsException>();

            exception.StatusCode.ShouldBe(StatusCodes.Status403Forbidden);
        }
    }
}

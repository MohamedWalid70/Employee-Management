using AutoFixture;
using Internship.EmployeeManagement.Api.IntegrationTests.NSwag.Generated;
using Internship.EmployeeManagement.Api.IntegrationTests.TestingHelpers;
using Internship.EmployeeManagement.Application.Features.Employees.Commands.UpdateEmployee;
using Microsoft.AspNetCore.Http;
using Shouldly;
using System.Net;

namespace Internship.EmployeeManagement.Api.IntegrationTests.Employees
{
    public partial class EmployeeManagementApiTests
    {
        [Fact]
        public async Task UpdateEmployee_WithValidInput_ShouldReturn200()
        {
            await HelperMethods.AuthenticateAdmin(_client, _fixture, _employeeManagementClientFixture);

            var employee = _fixture.Build<CreateEmployeeCommandParam>().Create();
            var creationResponse = await _client.EmployeesPOSTAsync(employee);

            var updateEmployeeData = _fixture.Build<UpdateEmployeeCommandParam>().With(x => x.Id, creationResponse.Result.Id).Create();

            var response = await _client.EmployeesPUTAsync(updateEmployeeData);

            var isMessageConsumed = _bus?.Consumed.Any(m => m == typeof(EmployeeUpdatedEvent));

            var getResponse = await _client.EmployeesGETAsync(updateEmployeeData.Id);

            isMessageConsumed?.ShouldBeTrue();
            response.StatusCode.ShouldBe(StatusCodes.Status200OK);
            getResponse.Result.Data.Title.ShouldBe(updateEmployeeData.Title);
            getResponse.Result.Data.Name.ShouldBe(updateEmployeeData.Name);
            getResponse.Result.Data.Age.ShouldBe(updateEmployeeData.Age);
        }

        [Fact]
        public async Task UpdateEmployee_WitOldMessageForReadDatabase_ShouldReturn500()
        {
            await HelperMethods.AuthenticateAdmin(_client, _fixture, _employeeManagementClientFixture);

            _bus?.UpdateEventOutdated = true;

            var employee = _fixture.Build<CreateEmployeeCommandParam>().Create();
            var creationResponse = await _client.EmployeesPOSTAsync(employee);

            var updateEmployeeData = _fixture.Build<UpdateEmployeeCommandParam>().With(x => x.Id, creationResponse.Result.Id).Create();

            var response = await _client.EmployeesPUTAsync(updateEmployeeData);

            var getResponse = await _client.EmployeesGETAsync(updateEmployeeData.Id);

            response.StatusCode.ShouldBe(StatusCodes.Status200OK);
            getResponse.Result.Data.Title.ShouldBe(employee.Title);
            getResponse.Result.Data.Name.ShouldBe(employee.Name);
            getResponse.Result.Data.Age.ShouldBe(employee.Age);

            _bus?.UpdateEventOutdated = false;
        }

        [Fact]
        public async Task UpdateEmployee_WhenEmployeeDoesNotExist_ShouldReturnShouldReturn404()
        {
            var invalidId = Guid.NewGuid();

            await HelperMethods.AuthenticateAdmin(_client, _fixture, _employeeManagementClientFixture);

            var updateEmployeeData = _fixture.Build<UpdateEmployeeCommandParam>().With(x => x.Id, invalidId).Create();

            var apiCall = async () => await _client.EmployeesPUTAsync(updateEmployeeData);

            var exception = await apiCall.ShouldThrowAsync<EmployeeManagementApiIntegrationTestsException>();

            exception.StatusCode.ShouldBe(StatusCodes.Status404NotFound);

        }

        [Fact]
        public async Task UpdateEmployee_WithUnauthenticatedAccess_ShouldReturn401()
        {
            _employeeManagementClientFixture.AuthorizationToken = "";

            var updateEmployeeData = _fixture.Build<UpdateEmployeeCommandParam>().Create();

            var apiCall = async () => await _client.EmployeesPUTAsync(updateEmployeeData);

            var exception = apiCall.ShouldThrow<EmployeeManagementApiIntegrationTestsException>();

            exception.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        public async Task UpdateEmployee_WithUserRoleValidInput_ShouldReturn200()
        {
            await CreateAndAuthenticateRandomUser();

            var employee = _fixture.Build<CreateEmployeeCommandParam>().Create();
            var creationResponse = await _client.EmployeesPOSTAsync(employee);

            var updateEmployeeData = _fixture.Build<UpdateEmployeeCommandParam>().With(x => x.Id, creationResponse.Result.Id).Create();

            var response = await _client.EmployeesPUTAsync(updateEmployeeData);

            response.StatusCode.ShouldBe(StatusCodes.Status200OK);

        }
    }
}

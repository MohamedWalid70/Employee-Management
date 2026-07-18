using AutoFixture;
using Internship.EmployeeManagement.Api.IntegrationTests.NSwag.Generated;
using Internship.EmployeeManagement.Api.IntegrationTests.TestingHelpers;
using Microsoft.AspNetCore.Http;
using Shouldly;

namespace Internship.EmployeeManagement.Api.IntegrationTests.Employees
{

    public partial class EmployeeManagementApiTests
    {

        [Fact]
        public async Task GetPaginatedEmployees_WithValidInput_ShouldReturn200WithTheList()
        {
            await HelperMethods.AuthenticateAdmin(_client, _fixture, _employeeManagementClientFixture);

            var employees = _fixture.Build<CreateEmployeeCommandParam>().CreateMany(2);
            SwaggerResponse<GuidIdResponse> creationResponse;

            foreach (var employee in employees)
            {
                creationResponse = await _client.EmployeesPOSTAsync(employee);
                creationResponse.StatusCode.ShouldBe(StatusCodes.Status201Created);
            }

            var response = await _client.EmployeesAllAsync(1, 2);

            response.StatusCode.ShouldBe(StatusCodes.Status200OK);
            response.Result.ShouldNotBeNull();
            response.Result.Count.ShouldBe(2);
        }


        [Fact]
        public async Task GetPaginatedEmployees_WithUnauthenticatedAccess_ShouldReturn200WithTheList()
        {
            await HelperMethods.AuthenticateAdmin(_client, _fixture, _employeeManagementClientFixture);

            _employeeManagementClientFixture.AuthorizationToken = "";

            var apiCall = async () => await _client.EmployeesAllAsync(1, 2);

            var exception = apiCall.ShouldThrow<EmployeeManagementApiIntegrationTestsException>();

            exception.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
        }


        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(0, 2)]
        [InlineData(-1, -3)]
        public async Task GetPaginatedEmployees_WithInvalidInput_ShouldReturnEmptyList(int pageNumber, int pageSize)
        {
            await HelperMethods.AuthenticateAdmin(_client, _fixture, _employeeManagementClientFixture);

            var employees = _fixture.Build<CreateEmployeeCommandParam>().CreateMany(2);
            SwaggerResponse<GuidIdResponse> creationResponse;

            foreach (var employee in employees)
            {
                creationResponse = await _client.EmployeesPOSTAsync(employee);
                creationResponse.StatusCode.ShouldBe(StatusCodes.Status201Created);
            }

            var response = await _client.EmployeesAllAsync(pageNumber, pageSize);

            response.Result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetPaginatedEmployees_WithoutPagingParameters_ShouldReturn200WithTheList()
        {
            await HelperMethods.AuthenticateAdmin(_client, _fixture, _employeeManagementClientFixture);

            var employees = _fixture.Build<CreateEmployeeCommandParam>().CreateMany(6);
            SwaggerResponse<GuidIdResponse> creationResponse;

            foreach (var employee in employees)
            {
                creationResponse = await _client.EmployeesPOSTAsync(employee);
                creationResponse.StatusCode.ShouldBe(StatusCodes.Status201Created);
            }

            var response = await _client.EmployeesAllAsync(null, null);

            response.StatusCode.ShouldBe(StatusCodes.Status200OK);
            response.Result.Count.ShouldBe(5);
        }

        [Fact]
        public async Task GetEmployeeDataById_WithValidInput_ShouldReturn200WithTheEmployee()
        {
            await HelperMethods.AuthenticateAdmin(_client, _fixture, _employeeManagementClientFixture);

            var employee = _fixture.Build<CreateEmployeeCommandParam>().Create();
            var creationResponse = await _client.EmployeesPOSTAsync(employee);
            var response = await _client.EmployeesGETAsync(creationResponse.Result.Id);

            response.StatusCode.ShouldBe(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task GetEmployeeDataById_WhenEmployeeDoesNotExist_ShouldReturn200AndNull()
        {
            var invalidId = Guid.NewGuid();

            await HelperMethods.AuthenticateAdmin(_client, _fixture, _employeeManagementClientFixture);

            var employee = _fixture.Build<CreateEmployeeCommandParam>().Create();
            var creationResponse = await _client.EmployeesPOSTAsync(employee);

            var response = await _client.EmployeesGETAsync(invalidId);

            response.StatusCode.ShouldBe(StatusCodes.Status200OK);
            response.Result.Data.ShouldBeNull();
        }

        [Fact]
        public async Task GetRelatedEmployeeHistoryByEmployeeId_WithValidId_ShouldReturn200WithTheEmployeeHistory()
        {
            await HelperMethods.AuthenticateAdmin(_client, _fixture, _employeeManagementClientFixture);

            var employee = _fixture.Build<CreateEmployeeCommandParam>().Create();
            var creationResponse = await _client.EmployeesPOSTAsync(employee);

            var returnedHistory = await _client.HistoryAsync(creationResponse.Result.Id);

            returnedHistory.StatusCode.ShouldBe(StatusCodes.Status200OK);
            returnedHistory.Result.Data.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task GetRelatedEmployeeHistoryByEmployeeId_WhenEmployeeDoesNotExist_ShouldReturn200AndEmptyList()
        {
            var invalidId = Guid.NewGuid();

            await HelperMethods.AuthenticateAdmin(_client, _fixture, _employeeManagementClientFixture);

            var employee = _fixture.Build<CreateEmployeeCommandParam>().Create();
            var creationResponse = await _client.EmployeesPOSTAsync(employee);

            var returnedHistory = await _client.HistoryAsync(invalidId);

            returnedHistory.StatusCode.ShouldBe(StatusCodes.Status200OK);
            returnedHistory.Result.Data.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetRelatedEmployeeHistoryByEmployeeId_WithUnauthenticatedAccess_ShouldReturn401()
        {
            var randomId = Guid.NewGuid();

            _employeeManagementClientFixture.AuthorizationToken = "";

            var apiCall = async () => await _client.HistoryAsync(randomId);

            var exception = apiCall.ShouldThrow<EmployeeManagementApiIntegrationTestsException>();

            exception.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        public async Task GetPaginatedEmployees_WithUserRoleWithValidInput_ShouldReturn403()
        {
            await CreateAndAuthenticateRandomUser();

            var employees = _fixture.Build<CreateEmployeeCommandParam>().CreateMany(2);
            SwaggerResponse<GuidIdResponse> creationResponse;

            foreach (var employee in employees)
            {
                creationResponse = await _client.EmployeesPOSTAsync(employee);
                creationResponse.StatusCode.ShouldBe(StatusCodes.Status201Created);
            }

            var apiCall = async () => await _client.EmployeesAllAsync(1, 2);

            var exception = await apiCall.ShouldThrowAsync<EmployeeManagementApiIntegrationTestsException>();

            exception.StatusCode.ShouldBe(StatusCodes.Status403Forbidden);

        }


        [Fact]
        public async Task GetEmployeeDataById_WithUserRoleAndValidInput_ShouldReturn403()
        {
            await CreateAndAuthenticateRandomUser();

            var employee = _fixture.Build<CreateEmployeeCommandParam>().Create();
            var creationResponse = await _client.EmployeesPOSTAsync(employee);

            var apiCall = async () => await _client.EmployeesGETAsync(creationResponse.Result.Id);

            var exception = await apiCall.ShouldThrowAsync<EmployeeManagementApiIntegrationTestsException>();

            exception.StatusCode.ShouldBe(StatusCodes.Status403Forbidden);
        }


        [Fact]
        public async Task GetRelatedEmployeeHistoryByEmployeeId_WithUserRoleAndValidId_ShouldReturn403()
        {
            await CreateAndAuthenticateRandomUser();

            var employee = _fixture.Build<CreateEmployeeCommandParam>().Create();
            var creationResponse = await _client.EmployeesPOSTAsync(employee);

            var apiCall = async () => await _client.HistoryAsync(creationResponse.Result.Id);

            var exception = await apiCall.ShouldThrowAsync<EmployeeManagementApiIntegrationTestsException>();

            exception.StatusCode.ShouldBe(StatusCodes.Status403Forbidden);
        }
    }
}
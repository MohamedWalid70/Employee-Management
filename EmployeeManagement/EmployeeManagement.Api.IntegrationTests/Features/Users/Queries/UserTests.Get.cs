using AutoFixture;
using Internship.EmployeeManagement.Api.IntegrationTests.NSwag.Generated;
using Internship.EmployeeManagement.Api.IntegrationTests.TestingHelpers;
using Microsoft.AspNetCore.Http;
using Shouldly;

namespace Internship.EmployeeManagement.Api.IntegrationTests.Users
{
    public partial class UserApiTests
    {
        [Fact]
        public async Task GetUserById_WithValidInputWhenAdminAccesses_ShouldReturn200WithTheTargetUser()
        {
            var user = _fixture.Create<CreateUserCommandParam>();

            var creationResponse = await _client.UsersPOSTAsync(user);

            await HelperMethods.AuthenticateAdmin(_client, _fixture, _employeeManagementClientFixture);

            var getResponse = await _client.UsersGETAsync(creationResponse.Result.Id);
            
            getResponse.StatusCode.ShouldBe(StatusCodes.Status200OK);
            getResponse.Result.Data.FirstName.ShouldBe(user.FirstName);
        }

        [Theory]
        [InlineData(999999)]
        public async Task GetUserById_WhenUserDoesNotExistWhenAdminAccesses_ShouldReturn404(int id)
        {
            var user = _fixture.Create<CreateUserCommandParam>();

            var creationResponse = await _client.UsersPOSTAsync(user);

            await HelperMethods.AuthenticateAdmin(_client, _fixture, _employeeManagementClientFixture);

            var getResponse = await _client.UsersGETAsync(id);

            getResponse.StatusCode.ShouldBe(StatusCodes.Status200OK);
            getResponse.Result.Data.ShouldBeNull();

        }


        [Fact]
        public async Task GetUserById_WithValidInputWhenUserAccesses_ShouldReturn403()
        {
            var user = _fixture.Create<CreateUserCommandParam>();

            var creationResponse = await _client.UsersPOSTAsync(user);

            var loginQueryParam = _fixture.Build<LoginQueryParam>()
                .With(x => x.Email, user.Email)
                .With(x => x.Password, user.Password)
                .Create();

            await HelperMethods.AuthenticateUser(loginQueryParam, _client, _fixture, _employeeManagementClientFixture);

            var apiCall = async () => await _client.UsersGETAsync(creationResponse.Result.Id);

            var response = await apiCall.ShouldThrowAsync<EmployeeManagementApiIntegrationTestsException>();

            response.StatusCode.ShouldBe(StatusCodes.Status403Forbidden);
        }

    }
}

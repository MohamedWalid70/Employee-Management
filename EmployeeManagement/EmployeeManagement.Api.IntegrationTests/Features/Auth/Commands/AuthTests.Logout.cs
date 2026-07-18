using AutoFixture;
using Internship.EmployeeManagement.Api.IntegrationTests.NSwag.Generated;
using Microsoft.AspNetCore.Http;
using Shouldly;

namespace Internship.EmployeeManagement.Api.IntegrationTests.Auth
{
    public partial class AuthTests
    {
        [Fact]
        public async Task Logout_WithValidInput_ShouldReturn200()
        {
            var loginQueryParam = _fixture.Build<LoginQueryParam>()
                    .With(x => x.Email, _seededUser?.Email)
                    .With(x => x.Password, _seededUser?.Password)
                    .Create();

            var response = await _client.AuthAsync(loginQueryParam);

            _employeeManagementClientFixture.AuthorizationToken = response.Result.Data.AccessToken;

            var logoutResponse = await _client.LogoutAsync();

            logoutResponse.StatusCode.ShouldBe(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task Logout_WithNoAccessToken_ShouldReturn400()
        {
            var loginQueryParam = _fixture.Build<LoginQueryParam>()
                    .With(x => x.Email, _seededUser?.Email)
                    .With(x => x.Password, _seededUser?.Password)
                    .Create();

            var response = await _client.AuthAsync(loginQueryParam);

            _employeeManagementClientFixture.AuthorizationToken = "";

            var logoutResponse = await _client.LogoutAsync();

            logoutResponse.StatusCode.ShouldBe(StatusCodes.Status200OK);
        }

    }
}

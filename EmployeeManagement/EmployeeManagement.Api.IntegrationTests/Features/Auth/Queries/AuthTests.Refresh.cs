using AutoFixture;
using Internship.EmployeeManagement.Api.IntegrationTests.NSwag.Generated;
using Microsoft.AspNetCore.Http;
using Shouldly;

namespace Internship.EmployeeManagement.Api.IntegrationTests.Auth
{
    public partial class AuthTests
    {
        [Fact]
        public async Task Refresh_WithValidInput_ShouldReturn200AndSecurityTokens()
        {
            var loginQueryParam = _fixture.Build<LoginQueryParam>()
                    .With(x => x.Email, _seededUser?.Email)
                    .With(x => x.Password, _seededUser?.Password)
                    .Create();

            var response = await _client.AuthAsync(loginQueryParam);

            response = await _client.RefreshAsync(response.Result.Data.RefreshToken);

            response.StatusCode.ShouldBe(StatusCodes.Status200OK);
            response.Result.Data.AccessToken.ShouldNotBeNullOrEmpty();
            response.Result.Data.RefreshToken.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task Refresh_WithIncorrectAccessToken_ShouldReturn400()
        {
            var loginQueryParam = _fixture.Build<LoginQueryParam>()
                    .With(x => x.Email, _seededUser?.Email)
                    .With(x => x.Password, _seededUser?.Password)
                    .Create();

            var response = await _client.AuthAsync(loginQueryParam);

            var refreshToken = _fixture.Build<String>().Create();

            var apiCall = async () => await _client.RefreshAsync(refreshToken);

            var exception = apiCall.ShouldThrow<EmployeeManagementApiIntegrationTestsException>();

            exception.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task Refresh_WithLatestRefreshToken_ShouldReturn200AndSecurityTokens()
        {
            var loginQueryParam = _fixture.Build<LoginQueryParam>()
                    .With(x => x.Email, _seededUser?.Email)
                    .With(x => x.Password, _seededUser?.Password)
                    .Create();

            var oldResponse = await _client.AuthAsync(loginQueryParam);

            var latestResponse = await _client.AuthAsync(loginQueryParam);

            var response = await _client.RefreshAsync(latestResponse.Result.Data.RefreshToken);

            response.StatusCode.ShouldBe(StatusCodes.Status200OK);
            response.Result.Data.AccessToken.ShouldNotBeNullOrEmpty();
            response.Result.Data.RefreshToken.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task Refresh_WithOldRefreshToken_ShouldReturn400()
        {
            var loginQueryParam = _fixture.Build<LoginQueryParam>()
                    .With(x => x.Email, _seededUser?.Email)
                    .With(x => x.Password, _seededUser?.Password)
                    .Create();

            var oldResponse = await _client.AuthAsync(loginQueryParam);

            var latestResponse = await _client.AuthAsync(loginQueryParam);

            _employeeManagementClientFixture.AuthorizationToken = "";

            var apiCall = async () => await _client.RefreshAsync(oldResponse.Result.Data.RefreshToken);

            var exception = apiCall.ShouldThrow<EmployeeManagementApiIntegrationTestsException>();

            exception.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task Refresh_WithRefreshTokenAfterLogout_ShouldReturn400()
        {
            var loginQueryParam = _fixture.Build<LoginQueryParam>()
                    .With(x => x.Email, _seededUser?.Email)
                    .With(x => x.Password, _seededUser?.Password)
                    .Create();

            var response = await _client.AuthAsync(loginQueryParam);

            _employeeManagementClientFixture.AuthorizationToken = response.Result.Data.AccessToken;

            await _client.LogoutAsync();

            _employeeManagementClientFixture.AuthorizationToken = "";

            var apiCall = async () => await _client.RefreshAsync(response.Result.Data.RefreshToken);

            var exception = apiCall.ShouldThrow<EmployeeManagementApiIntegrationTestsException>();

            exception.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task Refresh_WithOldRefreshTokenAfterLogout_ShouldReturn400()
        {
            var loginQueryParam = _fixture.Build<LoginQueryParam>()
                    .With(x => x.Email, _seededUser?.Email)
                    .With(x => x.Password, _seededUser?.Password)
                    .Create();

            var oldResponse = await _client.AuthAsync(loginQueryParam);

            var latestResponse = await _client.AuthAsync(loginQueryParam);

            _employeeManagementClientFixture.AuthorizationToken = latestResponse.Result.Data.AccessToken;

            await _client.LogoutAsync();

            _employeeManagementClientFixture.AuthorizationToken = "";

            var apiCall = async () => await _client.RefreshAsync(oldResponse.Result.Data.RefreshToken);

            var exception = apiCall.ShouldThrow<EmployeeManagementApiIntegrationTestsException>();

            exception.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task Refresh_WithValidAccessToken_ShouldReturn200AndTheSameAccessAndRefreshToken()
        {
            var loginQueryParam = _fixture.Build<LoginQueryParam>()
                    .With(x => x.Email, _seededUser?.Email)
                    .With(x => x.Password, _seededUser?.Password)
                    .Create();

            var response = await _client.AuthAsync(loginQueryParam);

            _employeeManagementClientFixture.AuthorizationToken = response.Result.Data.AccessToken;

            response = await _client.RefreshAsync(response.Result.Data.RefreshToken);

            response.StatusCode.ShouldBe(StatusCodes.Status200OK);
            response.Result.Data.AccessToken.ShouldBe(response.Result.Data.AccessToken);
            response.Result.Data.RefreshToken.ShouldBe(response.Result.Data.RefreshToken);
        }
    }
}

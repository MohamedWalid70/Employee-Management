using AutoFixture;
using Internship.EmployeeManagement.Api.IntegrationTests.NSwag.Generated;
using Microsoft.AspNetCore.Http;
using Shouldly;

namespace Internship.EmployeeManagement.Api.IntegrationTests.Auth
{
    public partial class AuthTests
    {
        [Fact]
        public async Task Login_WithValidInput_ShouldReturn200AndSecurityTokens()
        {
            var loginQueryParam = _fixture.Build<LoginQueryParam>()
                    .With(x => x.Email, _seededUser?.Email)
                    .With(x => x.Password, _seededUser?.Password)
                    .Create();

            var response = await _client.AuthAsync(loginQueryParam);

            response.StatusCode.ShouldBe(StatusCodes.Status200OK);
            response.Result.Data.AccessToken.ShouldNotBeNullOrEmpty();
            response.Result.Data.RefreshToken.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_WithWrongEmail_ShouldReturn400()
        {

            var loginQueryParam = _fixture.Build<LoginQueryParam>()
                    .With(x => x.Email, "9999212212@gmail.com")
                    .With(x => x.Password, _seededUser?.Password)
                    .Create();

            var apiCall = async() => await _client.AuthAsync(loginQueryParam);

            var exception = apiCall.ShouldThrow<EmployeeManagementApiIntegrationTestsException>();

            exception.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task Login_WithWrongPassword_ShouldReturn400()
        {
            var loginQueryParam = _fixture.Build<LoginQueryParam>()
                    .With(x => x.Email, _seededUser?.Email)
                    .With(x => x.Password, "213434asd")
                    .Create();

            var apiCall = async () => await _client.AuthAsync(loginQueryParam);

            var exception = apiCall.ShouldThrow<EmployeeManagementApiIntegrationTestsException>();

            exception.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        }

    }

}

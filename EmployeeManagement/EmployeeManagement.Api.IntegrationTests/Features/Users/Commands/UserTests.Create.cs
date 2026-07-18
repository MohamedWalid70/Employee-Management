using AutoFixture;
using Internship.EmployeeManagement.Api.IntegrationTests.NSwag.Generated;
using Microsoft.AspNetCore.Http;
using Shouldly;

namespace Internship.EmployeeManagement.Api.IntegrationTests.Users
{
    public partial class UserApiTests
    {

        [Fact]
        public async Task CreateUser_WithValidInput_ShouldReturn201AndLocationHeader()
        {
            var user = _fixture.Create<CreateUserCommandParam>();

            var creationResponse = await _client.UsersPOSTAsync(user);

            creationResponse.StatusCode.ShouldBe(StatusCodes.Status201Created);
            creationResponse.Result.Id.ShouldBeGreaterThan(0);
            creationResponse.Headers.ShouldContain(x => x.Key == "Location");
        }

        [Fact]
        public async Task CreateUser_WithEmptyBody_ShouldReturn400()
        {
            var apiCall = async () => await _client.UsersPOSTAsync(null);

            var exception = await apiCall.ShouldThrowAsync<EmployeeManagementApiIntegrationTestsException>();
            exception.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        }
    }
}

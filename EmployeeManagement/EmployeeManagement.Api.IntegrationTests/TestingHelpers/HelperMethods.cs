using AutoFixture;
using Internship.EmployeeManagement.Api.IntegrationTests.NSwag.Generated;

namespace Internship.EmployeeManagement.Api.IntegrationTests.TestingHelpers
{
    internal class HelperMethods
    {
        internal static async Task AuthenticateAdmin(EmployeeManagementClient employeeManagementClient, 
            IFixture fixture, 
            EmployeeManagementClientFixture employeeManagementClientFixture)
        {
            var loginQueryParam = fixture.Build<LoginQueryParam>()
                .With(x => x.Email, "Admin@gmail.com")
                .With(x => x.Password, "123456Aa$")
                .Create();

            await AuthenticateUser(loginQueryParam, employeeManagementClient, fixture, employeeManagementClientFixture);
        }

        internal static async Task AuthenticateUser(LoginQueryParam loginQueryParam,
            EmployeeManagementClient employeeManagementClient,
            IFixture fixture,
            EmployeeManagementClientFixture employeeManagementClientFixture)
        {

            var loginResponse = await employeeManagementClient.AuthAsync(loginQueryParam);

            employeeManagementClientFixture.AuthorizationToken = loginResponse.Result.Data.AccessToken ?? "";
        }
    }
}

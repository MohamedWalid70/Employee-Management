using Internship.EmployeeManagement.Application.Features.Auth.Queries.Login;
using Internship.EmployeeManagement.Application.Features.Auth.Queries.RefreshToken;
using Internship.EmployeeManagement.Application.Features.Employees.Commands;
using Internship.EmployeeManagement.Application.Features.Employees.Commands.DeleteEmployee;
using Internship.EmployeeManagement.Application.Features.Employees.Commands.UpdateEmployee;
using Internship.EmployeeManagement.Application.Tokens;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Internship.EmployeeManagement.Application.Extensions
{
    public static class ServicesExtensions
    {
        public static void ResisterApplicationServices(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

            builder.Services.AddMediatR(
                config => {
                    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                    config.AddBehavior<ValidateEmployeeExistenceBehavoiur<UpdateEmployeeCommand, Unit>>();
                    config.AddBehavior<ValidateEmployeeExistenceBehavoiur<DeleteEmployeeCommand, Unit>>();
                    config.AddBehavior<LoginQueryValidationBehaviour>();
                    config.AddBehavior<ValidateRefreshTokenBehaviour>();
                });
        }
    }
}

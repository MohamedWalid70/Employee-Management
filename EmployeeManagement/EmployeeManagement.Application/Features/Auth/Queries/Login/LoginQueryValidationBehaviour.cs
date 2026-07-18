using Internship.EmployeeManagement.Application.Exceptions;
using Internship.EmployeeManagement.Application.Features.Auth.Queries.Common;
using Internship.EmployeeManagement.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Internship.EmployeeManagement.Application.Features.Auth.Queries.Login
{
    public class LoginQueryValidationBehaviour(UserManager<UserEntity> userManager) : IPipelineBehavior<LoginQuery, AuthQueryResponse>
    {
        private readonly UserManager<UserEntity> _userManager = userManager;

        public async Task<AuthQueryResponse> Handle(LoginQuery request, RequestHandlerDelegate<AuthQueryResponse> next, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
                throw new BadRequestException("Invalid email or password!");

            var result = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!result)
                throw new BadRequestException("Invalid username or password");

            request.SharedUser = user;

            var response = await next();

            return response;
        }
    }
}

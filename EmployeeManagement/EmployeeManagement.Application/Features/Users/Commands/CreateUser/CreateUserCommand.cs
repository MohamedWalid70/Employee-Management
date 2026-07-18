using Internship.EmployeeManagement.Application.Features.Common;
using MediatR;

namespace Internship.EmployeeManagement.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<IdResponse<int>>
    {
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Password { get; set; }
    }
}

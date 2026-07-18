using MediatR;

namespace Internship.EmployeeManagement.Application.Features.Auth.Commands.Logout
{
    public record LogoutCommand() : IRequest<Unit>;
}

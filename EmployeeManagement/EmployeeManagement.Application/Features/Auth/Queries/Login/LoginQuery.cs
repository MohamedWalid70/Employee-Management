using Internship.EmployeeManagement.Application.Features.Auth.Queries.Common;
using Internship.EmployeeManagement.Core.Entities;
using MediatR;

namespace Internship.EmployeeManagement.Application.Features.Auth.Queries.Login
{
    public class LoginQuery: IRequest<AuthQueryResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public UserEntity SharedUser { get; set; }
    }
}

using Internship.EmployeeManagement.Application.Features.Auth.Queries.Common;
using Internship.EmployeeManagement.Core.Entities;
using MediatR;

namespace Internship.EmployeeManagement.Application.Features.Auth.Queries.RefreshToken
{
    public class RefreshTokenQuery : IRequest<AuthQueryResponse>
    {
        public string RefreshToken { get; set; }
        public RefreshTokenEntity SharedExistentToken { get; set; }
    }
}

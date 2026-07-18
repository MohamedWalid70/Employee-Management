using MediatR;

namespace Internship.EmployeeManagement.Application.Features.Users.Queries.GetUserById
{
    public record GetUserByIdQuery(int Id) : IRequest<GetUserByIdQueryResponse?>;
}

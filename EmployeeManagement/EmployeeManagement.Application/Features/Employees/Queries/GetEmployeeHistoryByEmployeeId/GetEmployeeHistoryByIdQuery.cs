using MediatR;

namespace Internship.EmployeeManagement.Application.Features.Employees.Queries.GetEmployeeHistoryByEmployeeId
{
    public record GetEmployeeHistoryByIdQuery(Guid Id) : IRequest<IEnumerable<GetEmployeeHistoryByIdQueryResponse>>;
}

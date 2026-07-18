using MediatR;

namespace Internship.EmployeeManagement.Application.Features.Employees.Queries.GetEmployeeById
{
    public record GetEmployeeByIdQuery(Guid Id) : IRequest<GetEmployeeByIdQueryResponse?>;
    
}

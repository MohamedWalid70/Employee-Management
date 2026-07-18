using Internship.EmployeeManagement.Application.Features.Employees.Queries.GetEmployeeById;
using MediatR;

namespace Internship.EmployeeManagement.Application.Features.Employees.Queries.GetPaginatedEmployees
{
    public record GetPaginatedEmployeesQuery(int PageNumber, int PageSize) : IStreamRequest<GetEmployeeByIdQueryResponse>;
}

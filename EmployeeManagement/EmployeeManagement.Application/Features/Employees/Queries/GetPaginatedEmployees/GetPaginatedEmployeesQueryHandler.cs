using AutoMapper;
using Internship.EmployeeManagement.Application.Features.Employees.Queries.GetEmployeeById;
using Internship.EmployeeManagement.Core.Interfaces;
using Internship.EmployeeManagement.Core.Interfaces.Employee;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Internship.EmployeeManagement.Application.Features.Employees.Queries.GetPaginatedEmployees
{
    public class GetPaginatedEmployeesQueryHandler(IEmployeeRepository<IReadDbContext> repository, IMapper mapper) : IStreamRequestHandler<GetPaginatedEmployeesQuery, GetEmployeeByIdQueryResponse>
    {
        private readonly IEmployeeRepository<IReadDbContext> _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async IAsyncEnumerable<GetEmployeeByIdQueryResponse> Handle(GetPaginatedEmployeesQuery request, [EnumeratorCancellation]CancellationToken cancellationToken)
        {
            if (request.PageNumber > 0 && request.PageSize > 0)
            {

                var employees = _repository.GetPaginatedEmployeesAsync(request.PageNumber - 1, request.PageSize);

                await foreach (var item in employees)
                {
                    yield return _mapper.Map<GetEmployeeByIdQueryResponse>(item);
                }
            }
        }
    }
}

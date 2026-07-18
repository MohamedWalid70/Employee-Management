using AutoMapper;
using Internship.EmployeeManagement.Core.Interfaces;
using Internship.EmployeeManagement.Core.Interfaces.Employee;
using MediatR;

namespace Internship.EmployeeManagement.Application.Features.Employees.Queries.GetEmployeeById
{
    public class GetEmployeeByIdHandler(IEmployeeRepository<IReadDbContext> repository, IMapper mapper) : IRequestHandler<GetEmployeeByIdQuery, GetEmployeeByIdQueryResponse?>
    {
        private readonly IEmployeeRepository<IReadDbContext> _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<GetEmployeeByIdQueryResponse?> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            var employee = await _repository.GetEmployeeByIdAsync(request.Id);

            var employeeQueryParam = _mapper.Map<GetEmployeeByIdQueryResponse?>(employee);

            return employeeQueryParam;
        }
    }
}

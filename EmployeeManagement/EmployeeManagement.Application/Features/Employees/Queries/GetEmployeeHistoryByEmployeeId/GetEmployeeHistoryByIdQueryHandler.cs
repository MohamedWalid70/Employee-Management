using AutoMapper;
using Internship.EmployeeManagement.Application.CustomTypes;
using Internship.EmployeeManagement.Core.Interfaces.Employee;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Internship.EmployeeManagement.Application.Features.Employees.Queries.GetEmployeeHistoryByEmployeeId
{
    public class GetEmployeeHistoryByIdQueryHandler([FromKeyedServices(ContextType.Read)] IEmployeeHistory employeeHistory, IMapper mapper) : IRequestHandler<GetEmployeeHistoryByIdQuery, IEnumerable<GetEmployeeHistoryByIdQueryResponse>>
    {
        private readonly IEmployeeHistory _employeeHistory  = employeeHistory;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<GetEmployeeHistoryByIdQueryResponse>> Handle(GetEmployeeHistoryByIdQuery request, CancellationToken cancellationToken)
        {
            var history = await _employeeHistory.GetEmployeeHistoryByEmployeeIdAsync(request.Id);

            var getEmployeeRecordQueryResponsesList = _mapper.Map<IEnumerable<GetEmployeeHistoryByIdQueryResponse>>(history);

            return getEmployeeRecordQueryResponsesList;
        }
    }
}

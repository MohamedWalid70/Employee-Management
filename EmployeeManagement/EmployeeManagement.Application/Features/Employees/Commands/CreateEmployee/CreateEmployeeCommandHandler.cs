using AutoMapper;
using Internship.EmployeeManagement.Application.Features.Common;
using Internship.EmployeeManagement.Core.Entities;
using Internship.EmployeeManagement.Core.Interfaces;
using Internship.EmployeeManagement.Core.Interfaces.Employee;
using MediatR;

namespace Internship.EmployeeManagement.Application.Features.Employees.Commands.CreateEmployee
{
    public class CreateEmployeeCommandHandler(IEmployeeRepository<IWriteDbContext> repository , IWriteDbContext dbContext, IMapper mapper, IEventBus eventBus) : IRequestHandler<CreateEmployeeCommand, IdResponse<Guid>>
    {
        private readonly IEmployeeRepository<IWriteDbContext> _repository = repository;
        private readonly IWriteDbContext _dbContext = dbContext;
        private IMapper _mapper = mapper;
        private readonly IEventBus _eventBus = eventBus;

        public async Task<IdResponse<Guid>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = _mapper.Map<EmployeeEntity>(request);

            await _repository.AddEmployeeAsync(employee);

            await _dbContext.SaveChangesAsync();

            var employeeCreatedEvent = _mapper.Map<EmployeeCreatedEvent>(employee);

            await _eventBus.PublishAsync(employeeCreatedEvent, cancellationToken);

            return  new IdResponse<Guid> { Id = employee.Id };
        }
    }
}

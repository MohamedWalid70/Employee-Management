using AutoMapper;
using Internship.EmployeeManagement.Application.CustomTypes;
using Internship.EmployeeManagement.Core.Interfaces;
using Internship.EmployeeManagement.Core.Interfaces.Employee;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Internship.EmployeeManagement.Application.Features.Employees.Commands.UpdateEmployee
{
    public class UpdateEmployeeCommandHandler(IWriteDbContext dbContext, IMapper mapper, IEventBus eventBus) : IRequestHandler<UpdateEmployeeCommand, Unit>
    {
        private readonly IWriteDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;
        private readonly IEventBus _eventBus = eventBus;

        public async Task<Unit> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            _mapper.Map(request, request.SharedObject);

            await _dbContext.SaveChangesAsync();

            var updateDateTime = DateTime.UtcNow;

            var employeeUpdatedEvent = _mapper.Map<EmployeeUpdatedEvent>(request.SharedObject);

            employeeUpdatedEvent.UpdateDateTime = updateDateTime;

            await _eventBus.PublishAsync(employeeUpdatedEvent, cancellationToken);

            return Unit.Value;
        }
    }
}

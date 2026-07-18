using Internship.EmployeeManagement.Core.Interfaces;
using Internship.EmployeeManagement.Core.Interfaces.Employee;
using MediatR;

namespace Internship.EmployeeManagement.Application.Features.Employees.Commands.DeleteEmployee
{
    public class DeleteEmployeeCommandHandler(IEmployeeRepository<IWriteDbContext> repository, IWriteDbContext dbContext, IEventBus eventBus) : IRequestHandler<DeleteEmployeeCommand, Unit>
    {
        private readonly IEmployeeRepository<IWriteDbContext> _repository = repository;
        private readonly IWriteDbContext _dbContext = dbContext;
        private readonly IEventBus _eventBus = eventBus;

        public async Task<Unit> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            _repository.RemoveEmployee(request.SharedObject);

            await _dbContext.SaveChangesAsync();

            EmployeeDeleteEvent employeeDeleteEvent = new(request.EntityId);

            await _eventBus.PublishAsync(employeeDeleteEvent, cancellationToken);

            return Unit.Value;
        }
    }
}

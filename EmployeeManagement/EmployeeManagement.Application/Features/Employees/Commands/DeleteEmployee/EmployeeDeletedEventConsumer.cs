using Internship.EmployeeManagement.Core.Interfaces;
using Internship.EmployeeManagement.Core.Interfaces.Employee;
using MassTransit;

namespace Internship.EmployeeManagement.Application.Features.Employees.Commands.DeleteEmployee
{
    public class EmployeeDeletedEventConsumer(IEmployeeRepository<IReadDbContext> repository, IReadDbContext dbContext) : IConsumer<EmployeeDeleteEvent>
    {
        private readonly IEmployeeRepository<IReadDbContext> _repository = repository;
        private readonly IReadDbContext _dbContext = dbContext;

        public async Task Consume(ConsumeContext<EmployeeDeleteEvent> context)
        {
            await _repository.RemoveEmployeeByIdAsync(context.Message.Id);
        }
    }
}

using AutoMapper;
using Internship.EmployeeManagement.Core.Entities;
using Internship.EmployeeManagement.Core.Interfaces;
using Internship.EmployeeManagement.Core.Interfaces.Employee;
using MassTransit;

namespace Internship.EmployeeManagement.Application.Features.Employees.Commands.CreateEmployee
{
    public class EmployeeCreatedEventConsumer(IEmployeeRepository<IReadDbContext> repository, IReadDbContext dbContext, IMapper mapper) : IConsumer<EmployeeCreatedEvent>
    {
        private readonly IEmployeeRepository<IReadDbContext> _repository = repository;
        private readonly IReadDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;

        public async Task Consume(ConsumeContext<EmployeeCreatedEvent> context)
        {
            if (await _repository.DoesEmployeeExist(context.Message.Id))
                return;

            var employeeEntity = _mapper.Map<EmployeeEntity>(context.Message);

            await _repository.AddEmployeeAsync(employeeEntity);

            await _dbContext.SaveChangesAsync();
        }
    }
}

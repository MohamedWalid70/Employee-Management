using AutoMapper;
using Internship.EmployeeManagement.Application.CustomTypes;
using Internship.EmployeeManagement.Core.Interfaces;
using Internship.EmployeeManagement.Core.Interfaces.Employee;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Internship.EmployeeManagement.Application.Features.Employees.Commands.UpdateEmployee
{
    public class EmployeeUpdatedEventConsumer(
        IEmployeeRepository<IReadDbContext> repository, 
        IReadDbContext dbContext, 
        [FromKeyedServices(ContextType.Read)] IEmployeeHistory readHistory,
        IMapper mapper) : IConsumer<EmployeeUpdatedEvent>
    {
        private readonly IEmployeeRepository<IReadDbContext> _repository = repository;
        private readonly IReadDbContext _dbContext = dbContext;
        private readonly IEmployeeHistory _readHistory = readHistory;
        private readonly IMapper _mapper = mapper;

        public async Task Consume(ConsumeContext<EmployeeUpdatedEvent> context)
        {
            var readRecentChange = await _readHistory.GetLastEmployeeHistoryRecord(context.Message.Id);

            if (readRecentChange is null)
                throw new ArgumentNullException(nameof(readRecentChange));

            var isMessageOld = readRecentChange.CreationDateTime > context.Message.UpdateDateTime;

            if (isMessageOld)
                return;

            var employeeEntity = await _repository.GetEmployeeByIdAsync(context.Message.Id);

            _mapper.Map(context.Message, employeeEntity);

            await _dbContext.SaveChangesAsync();
        }
    }
}

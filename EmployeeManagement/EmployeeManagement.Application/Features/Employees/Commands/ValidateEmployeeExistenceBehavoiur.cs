using Internship.EmployeeManagement.Application.Exceptions;
using Internship.EmployeeManagement.Core.Entities;
using Internship.EmployeeManagement.Core.Interfaces;
using Internship.EmployeeManagement.Core.Interfaces.Employee;
using MediatR;

namespace Internship.EmployeeManagement.Application.Features.Employees.Commands
{
    public class ValidateEmployeeExistenceBehavoiur<TRequest, TResponse>(IEmployeeRepository<IWriteDbContext> repository, IWriteDbContext dbContext) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>, IRequestValidation<EmployeeEntity>
    {
        private readonly IEmployeeRepository<IWriteDbContext> _repository = repository;
        private readonly IWriteDbContext _dbContext = dbContext;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var employee = await _repository.GetEmployeeByIdAsync(request.EntityId);

            if (employee == null)
            {
                throw new NotFoundException("The specified id doesn't match any entry");
            }

            request.SharedObject = employee;

            var response = await next();

            return response;
        }
    }
}


using Internship.EmployeeManagement.Core.Entities;
using Internship.EmployeeManagement.Core.Interfaces;
using MediatR;

namespace Internship.EmployeeManagement.Application.Features.Employees.Commands.DeleteEmployee
{
    public class DeleteEmployeeCommand : IRequest<Unit>, IRequestValidation<EmployeeEntity>
    {
        public Guid EntityId { get; set; }
        public EmployeeEntity? SharedObject { get; set; }

    }

}

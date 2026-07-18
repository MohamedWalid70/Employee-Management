using Internship.EmployeeManagement.Core.Entities;
using Internship.EmployeeManagement.Core.Interfaces;
using MediatR;

namespace Internship.EmployeeManagement.Application.Features.Employees.Commands.UpdateEmployee
{
    public class UpdateEmployeeCommand : IRequest<Unit>, IRequestValidation<EmployeeEntity>
    {
        public Guid EntityId { get; set; }
        public string Name { get; set; }
        public byte Age { get; set; }
        public string Title { get; set; }
        public EmployeeEntity? SharedObject { get; set; }

    }
}

using Internship.EmployeeManagement.Application.Features.Common;
using MediatR;

namespace Internship.EmployeeManagement.Application.Features.Employees.Commands.CreateEmployee
{
    public record CreateEmployeeCommand : IRequest<IdResponse<Guid>>
    {
        public string Name { get; set; }
        public byte Age { get; set; }
        public string Title { get; set; }
    }
}


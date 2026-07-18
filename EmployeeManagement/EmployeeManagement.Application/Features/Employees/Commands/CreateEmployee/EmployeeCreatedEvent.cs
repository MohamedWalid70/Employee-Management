namespace Internship.EmployeeManagement.Application.Features.Employees.Commands.CreateEmployee
{
    public record EmployeeCreatedEvent(Guid Id, string Name, byte Age, string Title);
}

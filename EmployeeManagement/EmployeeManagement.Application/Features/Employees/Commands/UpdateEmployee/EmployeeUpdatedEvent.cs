namespace Internship.EmployeeManagement.Application.Features.Employees.Commands.UpdateEmployee
{
    public class EmployeeUpdatedEvent
    {
        public Guid Id { set; get; }
        public string Name { set; get; }
        public string Title { set; get; }
        public byte Age { set; get; }
        public DateTime UpdateDateTime { set; get; }
    }
}

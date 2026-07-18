namespace Internship.EmployeeManagement.Application.Features.Employees.Queries.GetEmployeeById
{
    public class GetEmployeeByIdQueryResponse 
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public byte Age { get; set; }
        public required string Title { get; set; }
    }
}

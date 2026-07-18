namespace Internship.EmployeeManagement.Application.Features.Employees.Queries.GetEmployeeHistoryByEmployeeId
{
    public class GetEmployeeHistoryByIdQueryResponse
    {
        public string? OperationType { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public required string Name { get; set; }
        public byte Age { get; set; }
        public required string Title { get; set; }
    }
}

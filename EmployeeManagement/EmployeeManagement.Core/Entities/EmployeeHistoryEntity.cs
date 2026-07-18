namespace Internship.EmployeeManagement.Core.Entities
{
    public class EmployeeHistoryEntity
    {
        public Guid Id { get; set; }
        public string? OperationType { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Name { get; set; }
        public byte Age { get; set; }
        public string Title { get; set; }
        public Guid EmployeeId { get; set; }
    }
}

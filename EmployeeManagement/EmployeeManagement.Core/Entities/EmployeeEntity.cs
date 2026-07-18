namespace Internship.EmployeeManagement.Core.Entities
{
    public class EmployeeEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public byte Age { get; set; }
        public required string Title { get; set; }
        public override string ToString()
        {
            return $"Name: {Name}, Age: {Age}, Title: {Title}";
        }
    }
}

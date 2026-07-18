namespace Internship.StringParser
{
    public class Employee
    {
        public required string Name { get; set; }
        public byte? Age { get; set; }
        public required string Title { get; set; }
        public bool FirstTime { get; set; }
        public override string ToString()
        {
            return $"Name: {Name}, Age: {Age}, Title: {Title}";
        }
    }
}

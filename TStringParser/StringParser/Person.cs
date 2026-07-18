namespace Internship.StringParser
{
    public class Person
    {
        public string? Name { get; set; }
        public bool IsMarried { get; set; }
        public string? Address { get; set; }
        public decimal TotalAccountBalance { get; set; }
        public int Age { get; set; }
        public override string ToString()
        {
            return $"----- Name: {Name}, Is person married: {IsMarried}, Address: {Address}, \nTotal account balance: {TotalAccountBalance}, Age: {Age} -----\n";
        }
    }
}

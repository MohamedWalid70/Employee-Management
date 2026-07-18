// See https://aka.ms/new-console-template for more information
using Internship.StringParser;

var employees = StringParser<Employee>.Parse("Name,Age,Title\r\nEmployee1,27, Manager\r\nEmployee2,30, SeniorManager\r\nEmployee3,35, SeniorManager2");

var persons = StringParser<Person>.Parse("Name,Address,TotalAccountBalance,Age\r\nEleanor Rigby,101 Penny Lane Liverpool UK,1250500.75,72\r\nMarcus Thorne,452 Maple Drive Evanston IL,84200.00,38\r\nSora Tanaka,null,12450.50,26\r\nDr. Aris Thorne,789 Oak Terrace Portland OR,450000.10,54");

foreach (var emp in employees)
{
    Console.WriteLine(emp.ToString());
}

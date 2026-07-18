namespace Internship.EmployeeManagement.Application.Exceptions
{
    public class BadRequestException : Exception, IStatusCodeException
    {
        public int StatusCode { get; } = 400;

        public BadRequestException(string message) : base(message)
        {
        }
    }
}

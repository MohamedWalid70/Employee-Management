namespace Internship.EmployeeManagement.Application.Exceptions
{
    public class NotFoundException : Exception, IStatusCodeException
    {
        public int StatusCode { get; } = 404;

        public NotFoundException(string message) : base(message)
        {
        }
    }
}

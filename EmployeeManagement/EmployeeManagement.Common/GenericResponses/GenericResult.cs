namespace Internship.EmployeeManagement.Common.GenericResponses
{
    public class GenericResult<T> : GenericResult
    {
        public T? Data { get; set; }
    }

    public class GenericResult
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }

    }
}

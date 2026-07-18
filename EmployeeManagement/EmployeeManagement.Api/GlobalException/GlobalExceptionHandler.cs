using Internship.EmployeeManagement.Application.Exceptions;
using Internship.EmployeeManagement.Common.GenericResponses;
using Microsoft.AspNetCore.Diagnostics;

namespace Internship.EmployeeManagement.Api.GlobalException
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            GenericResult genericResult;

            if (exception is IStatusCodeException statusCodeException)
                genericResult = new() { Message = exception.Message, StatusCode = statusCodeException.StatusCode };
            else
                genericResult = new() { StatusCode = StatusCodes.Status500InternalServerError };

            httpContext.Response.StatusCode = genericResult.StatusCode;

            await httpContext.Response.WriteAsJsonAsync(genericResult, cancellationToken);

            return true;

        }
    }
}

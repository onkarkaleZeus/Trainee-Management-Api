using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Exceptions;
 
namespace TraineeManagement.Api.Middleware;
 

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger

) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;
 
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Error occurred: {Message}", exception.Message);
 
 
        // Handle multiple exceptions in one place
        var (statusCode, title) = exception switch
        {
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            AccessForbiddenException => (StatusCodes.Status403Forbidden, "Unauthorized to access the resource"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found"),
            FileNotFoundException => (StatusCodes.Status404NotFound, "Resource File Not Found"),
            FileTooLargeException => (StatusCodes.Status413PayloadTooLarge, "Payload exceeds threshold limit"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid Input"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };
 
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message
        };
 
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
 
        return true;
    }
}
 
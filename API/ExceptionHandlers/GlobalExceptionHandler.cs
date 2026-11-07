using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TestApi.ExceptionHandler
{
    public class GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "unhandled exception accured!");
            return await problemDetailsService.TryWriteAsync
            (
                new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    Exception = exception,
                    ProblemDetails = new ProblemDetails
                    {
                        Type = exception.GetType().Name,
                        Title = "an exception accured", //as it's global (no specific exception)
                        Detail = exception.Message
                    }
                }
            );
        }
    }
}

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            ArgumentNullException => (StatusCodes.Status400BadRequest, "Argument null exception."),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid argument."),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found."),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Access denied."),
            _ => (StatusCodes.Status500InternalServerError, "Internal server error.")
        };

        context.Response.StatusCode = statusCode;

        _logger.LogError(exception, "Exception: {Message}", exception.Message);

        var errorResponse = new
        {
            statusCode,
            error = _env.IsDevelopment() ? exception.Message : message,
            details = _env.IsDevelopment() ? exception.StackTrace : null
        };

        await context.Response.WriteAsJsonAsync(errorResponse);
    }
}
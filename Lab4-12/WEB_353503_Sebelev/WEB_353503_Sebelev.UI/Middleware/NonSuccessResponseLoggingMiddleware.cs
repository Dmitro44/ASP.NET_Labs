using Serilog;

namespace WEB_353503_Sebelev.UI.Middleware;

public class NonSuccessResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public NonSuccessResponseLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);
        
        var statusCode = context.Response.StatusCode;

        if (statusCode is < 200 or > 299)
        {
            Log.Information("request {Path} returns {StatusCode}",
                context.Request.Path,
                statusCode);
        }
    }
}
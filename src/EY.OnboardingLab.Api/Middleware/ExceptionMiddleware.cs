using System.Net;
using System.Text.Json;

namespace EY.OnboardingLab.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while processing request.");

            if (context.Response.HasStarted)
                throw;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var body = new
            {
                message = "Unhandled exception.",
                detail = _env.IsDevelopment() ? ex.Message : null
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(body));
        }
    }
}


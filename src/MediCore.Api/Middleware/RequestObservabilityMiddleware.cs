using System.Diagnostics;

namespace MediCore.Api.Middleware;

public sealed class RequestObservabilityMiddleware(RequestDelegate next, ILogger<RequestObservabilityMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue("X-Correlation-ID", out var supplied) && !string.IsNullOrWhiteSpace(supplied) ? supplied.ToString() : Guid.NewGuid().ToString("N");
        context.TraceIdentifier = correlationId;
        context.Response.Headers["X-Correlation-ID"] = correlationId;
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-Frame-Options"] = "DENY";
        context.Response.Headers["Referrer-Policy"] = "no-referrer";
        context.Response.Headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";

        var stopwatch = Stopwatch.StartNew();
        try { await next(context); }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation("HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs} ms. CorrelationId={CorrelationId}", context.Request.Method, context.Request.Path, context.Response.StatusCode, stopwatch.Elapsed.TotalMilliseconds, correlationId);
        }
    }
}

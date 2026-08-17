using System.Security.Claims;
using MediCore.Domain.Audit;
using MediCore.Infrastructure.Persistence;

namespace MediCore.Api.Middleware;

public sealed class AuditTrailMiddleware(RequestDelegate next)
{
    private static readonly HashSet<string> WriteMethods = new(StringComparer.OrdinalIgnoreCase) { "POST", "PUT", "PATCH", "DELETE" };

    public async Task InvokeAsync(HttpContext context, MediCoreDbContext dbContext)
    {
        await next(context);
        if (!WriteMethods.Contains(context.Request.Method) || context.Response.StatusCode >= 400) return;

        Guid? userId = null;
        var userIdValue = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdValue, out var parsed)) userId = parsed;

        var segments = context.Request.Path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries) ?? [];
        var entityName = segments.Length > 1 ? segments[1] : "platform";
        var entityId = context.Request.RouteValues.TryGetValue("id", out var routeId) ? routeId?.ToString() : null;
        var details = $"Path={context.Request.Path}; Status={context.Response.StatusCode}; CorrelationId={context.TraceIdentifier}";
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();

        dbContext.AuditLogs.Add(new AuditLog(userId, context.Request.Method, entityName, entityId, details, ipAddress));
        await dbContext.SaveChangesAsync(CancellationToken.None);
    }
}

using MediCore.Application.Audit;
using MediCore.Application.Identity;

namespace MediCore.Api.Endpoints;

public static class AuditEndpoints
{
    public static IEndpointRouteBuilder MapAuditEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/audit").WithTags("Audit")
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.Administrator, RoleNames.Auditor));
        group.MapGet("/logs", async (int? take, string? entityName, IAuditService service, CancellationToken ct) =>
            Results.Ok(await service.GetRecentAsync(take ?? 100, entityName, ct)));
        return endpoints;
    }
}

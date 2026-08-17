using MediCore.Application.Analytics;
using MediCore.Application.Identity;

namespace MediCore.Api.Endpoints;

public static class AnalyticsEndpoints
{
    public static IEndpointRouteBuilder MapAnalyticsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/analytics").WithTags("Analytics")
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.Administrator, RoleNames.Doctor, RoleNames.Nurse, RoleNames.Pharmacist, RoleNames.Laboratory, RoleNames.Auditor));
        group.MapGet("/dashboard", async (IAnalyticsService service, CancellationToken ct) => Results.Ok(await service.GetDashboardAsync(ct)));
        group.MapGet("/inventory-alerts", async (IAnalyticsService service, CancellationToken ct) => Results.Ok(await service.GetInventoryAlertsAsync(ct)));
        return endpoints;
    }
}

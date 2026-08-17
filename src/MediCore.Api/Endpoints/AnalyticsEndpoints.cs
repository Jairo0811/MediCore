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
        group.MapGet("/operational-report", async (DateOnly? from, DateOnly? to, IAnalyticsService service, CancellationToken ct) =>
        {
            var resolvedTo = to ?? DateOnly.FromDateTime(DateTime.UtcNow);
            var resolvedFrom = from ?? resolvedTo.AddDays(-29);
            if (resolvedFrom > resolvedTo) return Results.BadRequest(new { error = "invalid_range", message = "La fecha inicial no puede ser posterior a la fecha final." });
            if (resolvedTo.DayNumber - resolvedFrom.DayNumber > 366) return Results.BadRequest(new { error = "range_too_large", message = "El rango máximo del reporte es de 366 días." });
            return Results.Ok(await service.GetOperationalReportAsync(resolvedFrom, resolvedTo, ct));
        });
        return endpoints;
    }
}

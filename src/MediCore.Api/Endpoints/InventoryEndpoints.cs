using System.Security.Claims;
using MediCore.Application.Common;
using MediCore.Application.Identity;
using MediCore.Application.Inventory;

namespace MediCore.Api.Endpoints;

public static class InventoryEndpoints
{
    public static IEndpointRouteBuilder MapInventoryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/inventory").WithTags("Inventory")
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.Administrator, RoleNames.Pharmacist, RoleNames.Doctor, RoleNames.Nurse, RoleNames.Auditor));

        group.MapGet("/lots", async (string? search, bool lowStockOnly, int? expiringWithinDays, IInventoryService service, CancellationToken ct) =>
            Results.Ok(await service.GetLotsAsync(search, lowStockOnly, expiringWithinDays, ct)));
        group.MapGet("/lots/{id:guid}/kardex", async (Guid id, IInventoryService service, CancellationToken ct) => Results.Ok(await service.GetKardexAsync(id, ct)));

        group.MapPost("/lots", async (CreateInventoryLotRequest request, HttpContext context, IInventoryService service, CancellationToken ct) =>
            ToResult(await service.CreateLotAsync(request, Actor(context), ct), true))
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.Administrator, RoleNames.Pharmacist));

        group.MapPost("/lots/{id:guid}/movements", async (Guid id, CreateInventoryMovementRequest request, HttpContext context, IInventoryService service, CancellationToken ct) =>
            ToResult(await service.RecordMovementAsync(id, request, Actor(context), ct), false))
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.Administrator, RoleNames.Pharmacist));
        return endpoints;
    }

    private static string Actor(HttpContext context) => context.User.FindFirstValue(ClaimTypes.Email) ?? context.User.Identity?.Name ?? context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";
    private static IResult ToResult<T>(OperationResult<T> result, bool created) => result.Succeeded
        ? created ? Results.Created(string.Empty, result.Value) : Results.Ok(result.Value)
        : result.ErrorCode switch
        {
            "not_found" => Results.NotFound(new { error = result.ErrorCode, message = result.ErrorMessage }),
            "lot_in_use" or "insufficient_stock" => Results.Conflict(new { error = result.ErrorCode, message = result.ErrorMessage }),
            _ => Results.BadRequest(new { error = result.ErrorCode, message = result.ErrorMessage })
        };
}

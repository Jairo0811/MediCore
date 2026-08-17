using System.Security.Claims;
using MediCore.Application.Common;
using MediCore.Application.Identity;
using MediCore.Application.Laboratory;

namespace MediCore.Api.Endpoints;

public static class LaboratoryEndpoints
{
    public static IEndpointRouteBuilder MapLaboratoryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/laboratory").WithTags("Laboratory")
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.Administrator, RoleNames.Doctor, RoleNames.Nurse, RoleNames.Laboratory, RoleNames.Auditor));
        group.MapGet("/tests", async (bool includeInactive, ILaboratoryService service, CancellationToken ct) => Results.Ok(await service.GetDefinitionsAsync(includeInactive, ct)));
        group.MapPost("/tests", async (CreateLabTestDefinitionRequest request, ILaboratoryService service, CancellationToken ct) => ToResult(await service.CreateDefinitionAsync(request, ct), true))
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.Administrator, RoleNames.Laboratory));
        group.MapGet("/orders", async (Guid? patientId, ILaboratoryService service, CancellationToken ct) => Results.Ok(await service.GetOrdersAsync(patientId, ct)));
        group.MapPost("/orders", async (CreateLabOrderRequest request, ILaboratoryService service, CancellationToken ct) => ToResult(await service.CreateOrderAsync(request, ct), true))
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.Administrator, RoleNames.Doctor));
        group.MapPost("/items/{id:guid}/result", async (Guid id, RecordLabResultRequest request, HttpContext context, ILaboratoryService service, CancellationToken ct) => ToResult(await service.RecordResultAsync(id, request, Actor(context), ct), false))
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.Administrator, RoleNames.Laboratory));
        return endpoints;
    }

    private static string Actor(HttpContext context) => context.User.FindFirstValue(ClaimTypes.Email) ?? context.User.Identity?.Name ?? context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";
    private static IResult ToResult<T>(OperationResult<T> result, bool created) => result.Succeeded ? created ? Results.Created(string.Empty, result.Value) : Results.Ok(result.Value) : result.ErrorCode == "not_found" ? Results.NotFound(new { error = result.ErrorCode, message = result.ErrorMessage }) : Results.BadRequest(new { error = result.ErrorCode, message = result.ErrorMessage });
}

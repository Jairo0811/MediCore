using MediCore.Application.Common;
using MediCore.Application.Identity;
using MediCore.Application.Staff;

namespace MediCore.Api.Endpoints;

public static class MedicalStaffEndpoints
{
    public static IEndpointRouteBuilder MapMedicalStaffEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/staff")
            .WithTags("Medical Staff")
            .RequireAuthorization(policy => policy.RequireRole(
                RoleNames.Administrator,
                RoleNames.Doctor,
                RoleNames.Nurse,
                RoleNames.Receptionist));

        group.MapGet("/", async (
            string? search,
            bool includeInactive,
            IMedicalStaffService service,
            CancellationToken cancellationToken) =>
            Results.Ok(await service.GetAllAsync(search, includeInactive, cancellationToken)));

        group.MapGet("/{id:guid}", async (
            Guid id,
            IMedicalStaffService service,
            CancellationToken cancellationToken) =>
        {
            var staff = await service.GetByIdAsync(id, cancellationToken);
            return staff is null ? Results.NotFound() : Results.Ok(staff);
        });

        group.MapPost("/", async (
            CreateMedicalStaffRequest request,
            IMedicalStaffService service,
            CancellationToken cancellationToken) =>
            ToResult(await service.CreateAsync(request, cancellationToken), created: true))
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.Administrator));

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateMedicalStaffRequest request,
            IMedicalStaffService service,
            CancellationToken cancellationToken) =>
            ToResult(await service.UpdateAsync(id, request, cancellationToken), created: false))
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.Administrator));

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IMedicalStaffService service,
            CancellationToken cancellationToken) =>
            ToResult(await service.DeactivateAsync(id, cancellationToken), created: false))
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.Administrator));

        return endpoints;
    }

    private static IResult ToResult<T>(OperationResult<T> result, bool created)
    {
        if (result.Succeeded)
        {
            return created ? Results.Created(string.Empty, result.Value) : Results.Ok(result.Value);
        }

        return result.ErrorCode switch
        {
            "not_found" => Results.NotFound(new { error = result.ErrorCode, message = result.ErrorMessage }),
            "cedula_in_use" => Results.Conflict(new { error = result.ErrorCode, message = result.ErrorMessage }),
            _ => Results.BadRequest(new { error = result.ErrorCode, message = result.ErrorMessage })
        };
    }
}

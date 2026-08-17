using MediCore.Application.Common;
using MediCore.Application.Identity;
using MediCore.Application.Patients;

namespace MediCore.Api.Endpoints;

public static class PatientEndpoints
{
    public static IEndpointRouteBuilder MapPatientEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/patients")
            .WithTags("Patients")
            .RequireAuthorization(policy => policy.RequireRole(
                RoleNames.Administrator,
                RoleNames.Doctor,
                RoleNames.Nurse,
                RoleNames.Receptionist));

        group.MapGet("/", async (
            string? search,
            bool includeInactive,
            IPatientService service,
            CancellationToken cancellationToken) =>
            Results.Ok(await service.GetAllAsync(search, includeInactive, cancellationToken)));

        group.MapGet("/{id:guid}", async (
            Guid id,
            IPatientService service,
            CancellationToken cancellationToken) =>
        {
            var patient = await service.GetByIdAsync(id, cancellationToken);
            return patient is null ? Results.NotFound() : Results.Ok(patient);
        });

        group.MapPost("/", async (
            CreatePatientRequest request,
            IPatientService service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.CreateAsync(request, cancellationToken);
            return ToResult(result, created: true);
        });

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdatePatientRequest request,
            IPatientService service,
            CancellationToken cancellationToken) =>
            ToResult(await service.UpdateAsync(id, request, cancellationToken), created: false));

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IPatientService service,
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

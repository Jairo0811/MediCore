using MediCore.Application.Common;
using MediCore.Application.Consultations;
using MediCore.Application.Identity;

namespace MediCore.Api.Endpoints;

public static class ConsultationEndpoints
{
    public static IEndpointRouteBuilder MapConsultationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/consultations")
            .WithTags("Consultations")
            .RequireAuthorization(policy => policy.RequireRole(
                RoleNames.Administrator,
                RoleNames.Doctor,
                RoleNames.Nurse));

        group.MapGet("/", async (
            Guid? patientId,
            Guid? medicalStaffId,
            DateTime? fromUtc,
            DateTime? toUtc,
            IConsultationService service,
            CancellationToken cancellationToken) =>
            Results.Ok(await service.GetAllAsync(
                patientId,
                medicalStaffId,
                fromUtc,
                toUtc,
                cancellationToken)));

        group.MapGet("/{id:guid}", async (
            Guid id,
            IConsultationService service,
            CancellationToken cancellationToken) =>
        {
            var consultation = await service.GetByIdAsync(id, cancellationToken);
            return consultation is null ? Results.NotFound() : Results.Ok(consultation);
        });

        group.MapPost("/", async (
            CreateConsultationRequest request,
            IConsultationService service,
            CancellationToken cancellationToken) =>
            ToResult(await service.CreateAsync(request, cancellationToken), created: true));

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateConsultationRequest request,
            IConsultationService service,
            CancellationToken cancellationToken) =>
            ToResult(await service.UpdateAsync(id, request, cancellationToken), created: false));

        group.MapPatch("/{id:guid}/status", async (
            Guid id,
            ChangeConsultationStatusRequest request,
            IConsultationService service,
            CancellationToken cancellationToken) =>
            ToResult(await service.ChangeStatusAsync(id, request, cancellationToken), created: false));

        endpoints.MapGet("/api/patients/{patientId:guid}/clinical-history", async (
            Guid patientId,
            IConsultationService service,
            CancellationToken cancellationToken) =>
        {
            var history = await service.GetClinicalHistoryAsync(patientId, cancellationToken);
            return history is null ? Results.NotFound() : Results.Ok(history);
        })
        .WithTags("Clinical History")
        .RequireAuthorization(policy => policy.RequireRole(
            RoleNames.Administrator,
            RoleNames.Doctor,
            RoleNames.Nurse));

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
            "not_found" or "patient_not_found" or "doctor_not_found" or "appointment_not_found" =>
                Results.NotFound(new { error = result.ErrorCode, message = result.ErrorMessage }),
            "appointment_mismatch" =>
                Results.Conflict(new { error = result.ErrorCode, message = result.ErrorMessage }),
            _ => Results.BadRequest(new { error = result.ErrorCode, message = result.ErrorMessage })
        };
    }
}

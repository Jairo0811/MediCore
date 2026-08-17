using MediCore.Application.Appointments;
using MediCore.Application.Common;
using MediCore.Application.Identity;

namespace MediCore.Api.Endpoints;

public static class AppointmentEndpoints
{
    public static IEndpointRouteBuilder MapAppointmentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/appointments")
            .WithTags("Appointments")
            .RequireAuthorization(policy => policy.RequireRole(
                RoleNames.Administrator,
                RoleNames.Doctor,
                RoleNames.Nurse,
                RoleNames.Receptionist));

        group.MapGet("/", async (
            DateTime? fromUtc,
            DateTime? toUtc,
            Guid? patientId,
            Guid? medicalStaffId,
            IAppointmentService service,
            CancellationToken cancellationToken) =>
            Results.Ok(await service.GetAllAsync(
                fromUtc,
                toUtc,
                patientId,
                medicalStaffId,
                cancellationToken)));

        group.MapGet("/{id:guid}", async (
            Guid id,
            IAppointmentService service,
            CancellationToken cancellationToken) =>
        {
            var appointment = await service.GetByIdAsync(id, cancellationToken);
            return appointment is null ? Results.NotFound() : Results.Ok(appointment);
        });

        group.MapPost("/", async (
            CreateAppointmentRequest request,
            IAppointmentService service,
            CancellationToken cancellationToken) =>
            ToResult(await service.CreateAsync(request, cancellationToken), created: true));

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateAppointmentRequest request,
            IAppointmentService service,
            CancellationToken cancellationToken) =>
            ToResult(await service.UpdateAsync(id, request, cancellationToken), created: false));

        group.MapPatch("/{id:guid}/status", async (
            Guid id,
            ChangeAppointmentStatusRequest request,
            IAppointmentService service,
            CancellationToken cancellationToken) =>
            ToResult(await service.ChangeStatusAsync(id, request, cancellationToken), created: false));

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
            "not_found" or "patient_not_found" or "doctor_not_found" =>
                Results.NotFound(new { error = result.ErrorCode, message = result.ErrorMessage }),
            "schedule_conflict" =>
                Results.Conflict(new { error = result.ErrorCode, message = result.ErrorMessage }),
            _ => Results.BadRequest(new { error = result.ErrorCode, message = result.ErrorMessage })
        };
    }
}

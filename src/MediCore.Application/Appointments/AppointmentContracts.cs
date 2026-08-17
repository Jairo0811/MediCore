using MediCore.Domain.Appointments;

namespace MediCore.Application.Appointments;

public sealed record CreateAppointmentRequest(
    Guid PatientId,
    Guid MedicalStaffId,
    DateTime ScheduledStartUtc,
    DateTime ScheduledEndUtc,
    string Reason,
    string? Notes);

public sealed record UpdateAppointmentRequest(
    Guid PatientId,
    Guid MedicalStaffId,
    DateTime ScheduledStartUtc,
    DateTime ScheduledEndUtc,
    string Reason,
    string? Notes);

public sealed record ChangeAppointmentStatusRequest(
    AppointmentStatus Status,
    string? CancellationReason);

public sealed record AppointmentResponse(
    Guid Id,
    Guid PatientId,
    string PatientName,
    string MedicalRecordNumber,
    Guid MedicalStaffId,
    string MedicalStaffName,
    string? Specialty,
    DateTime ScheduledStartUtc,
    DateTime ScheduledEndUtc,
    string Reason,
    string? Notes,
    AppointmentStatus Status,
    string? CancellationReason,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

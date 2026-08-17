using MediCore.Domain.Common;
using MediCore.Domain.Patients;
using MediCore.Domain.Staff;

namespace MediCore.Domain.Appointments;

public sealed class Appointment : BaseEntity
{
    private Appointment()
    {
    }

    public Appointment(
        Guid patientId,
        Guid medicalStaffId,
        DateTime scheduledStartUtc,
        DateTime scheduledEndUtc,
        string reason,
        string? notes)
    {
        PatientId = patientId;
        MedicalStaffId = medicalStaffId;
        ScheduledStartUtc = scheduledStartUtc;
        ScheduledEndUtc = scheduledEndUtc;
        Reason = reason;
        Notes = notes;
        Status = AppointmentStatus.Scheduled;
    }

    public Guid PatientId { get; private set; }
    public Guid MedicalStaffId { get; private set; }
    public DateTime ScheduledStartUtc { get; private set; }
    public DateTime ScheduledEndUtc { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public string? CancellationReason { get; private set; }

    public Patient Patient { get; private set; } = null!;
    public MedicalStaff MedicalStaff { get; private set; } = null!;

    public void Update(
        Guid patientId,
        Guid medicalStaffId,
        DateTime scheduledStartUtc,
        DateTime scheduledEndUtc,
        string reason,
        string? notes)
    {
        PatientId = patientId;
        MedicalStaffId = medicalStaffId;
        ScheduledStartUtc = scheduledStartUtc;
        ScheduledEndUtc = scheduledEndUtc;
        Reason = reason;
        Notes = notes;
        MarkAsUpdated();
    }

    public void ChangeStatus(AppointmentStatus status, string? cancellationReason = null)
    {
        Status = status;
        CancellationReason = status == AppointmentStatus.Cancelled
            ? cancellationReason
            : null;
        MarkAsUpdated();
    }
}

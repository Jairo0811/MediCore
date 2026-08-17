using MediCore.Domain.Appointments;
using MediCore.Domain.Common;
using MediCore.Domain.Patients;
using MediCore.Domain.Staff;

namespace MediCore.Domain.Consultations;

public sealed class Consultation : BaseEntity
{
    private Consultation()
    {
    }

    public Consultation(
        Guid patientId,
        Guid medicalStaffId,
        Guid? appointmentId,
        DateTime consultationDateUtc,
        string reason,
        string? symptoms,
        string? diagnosis,
        string? recommendations,
        string? notes,
        string? bloodPressure,
        decimal? temperatureCelsius,
        int? heartRate,
        decimal? weightKg,
        decimal? heightCm)
    {
        PatientId = patientId;
        MedicalStaffId = medicalStaffId;
        AppointmentId = appointmentId;
        ConsultationDateUtc = consultationDateUtc;
        Reason = reason;
        Symptoms = symptoms;
        Diagnosis = diagnosis;
        Recommendations = recommendations;
        Notes = notes;
        BloodPressure = bloodPressure;
        TemperatureCelsius = temperatureCelsius;
        HeartRate = heartRate;
        WeightKg = weightKg;
        HeightCm = heightCm;
        Status = ConsultationStatus.Draft;
    }

    public Guid PatientId { get; private set; }
    public Guid MedicalStaffId { get; private set; }
    public Guid? AppointmentId { get; private set; }
    public DateTime ConsultationDateUtc { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public string? Symptoms { get; private set; }
    public string? Diagnosis { get; private set; }
    public string? Recommendations { get; private set; }
    public string? Notes { get; private set; }
    public string? BloodPressure { get; private set; }
    public decimal? TemperatureCelsius { get; private set; }
    public int? HeartRate { get; private set; }
    public decimal? WeightKg { get; private set; }
    public decimal? HeightCm { get; private set; }
    public ConsultationStatus Status { get; private set; }

    public Patient Patient { get; private set; } = null!;
    public MedicalStaff MedicalStaff { get; private set; } = null!;
    public Appointment? Appointment { get; private set; }

    public void Update(
        DateTime consultationDateUtc,
        string reason,
        string? symptoms,
        string? diagnosis,
        string? recommendations,
        string? notes,
        string? bloodPressure,
        decimal? temperatureCelsius,
        int? heartRate,
        decimal? weightKg,
        decimal? heightCm)
    {
        ConsultationDateUtc = consultationDateUtc;
        Reason = reason;
        Symptoms = symptoms;
        Diagnosis = diagnosis;
        Recommendations = recommendations;
        Notes = notes;
        BloodPressure = bloodPressure;
        TemperatureCelsius = temperatureCelsius;
        HeartRate = heartRate;
        WeightKg = weightKg;
        HeightCm = heightCm;
        MarkAsUpdated();
    }

    public void Complete()
    {
        Status = ConsultationStatus.Completed;
        MarkAsUpdated();
    }

    public void Cancel()
    {
        Status = ConsultationStatus.Cancelled;
        MarkAsUpdated();
    }
}

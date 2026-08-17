using MediCore.Domain.Consultations;

namespace MediCore.Application.Consultations;

public sealed record CreateConsultationRequest(
    Guid PatientId,
    Guid MedicalStaffId,
    Guid? AppointmentId,
    DateTime ConsultationDateUtc,
    string Reason,
    string? Symptoms,
    string? Diagnosis,
    string? Recommendations,
    string? Notes,
    string? BloodPressure,
    decimal? TemperatureCelsius,
    int? HeartRate,
    decimal? WeightKg,
    decimal? HeightCm);

public sealed record UpdateConsultationRequest(
    DateTime ConsultationDateUtc,
    string Reason,
    string? Symptoms,
    string? Diagnosis,
    string? Recommendations,
    string? Notes,
    string? BloodPressure,
    decimal? TemperatureCelsius,
    int? HeartRate,
    decimal? WeightKg,
    decimal? HeightCm);

public sealed record ChangeConsultationStatusRequest(ConsultationStatus Status);

public sealed record ConsultationResponse(
    Guid Id,
    Guid PatientId,
    string PatientName,
    string MedicalRecordNumber,
    Guid MedicalStaffId,
    string MedicalStaffName,
    string? Specialty,
    Guid? AppointmentId,
    DateTime ConsultationDateUtc,
    string Reason,
    string? Symptoms,
    string? Diagnosis,
    string? Recommendations,
    string? Notes,
    string? BloodPressure,
    decimal? TemperatureCelsius,
    int? HeartRate,
    decimal? WeightKg,
    decimal? HeightCm,
    ConsultationStatus Status,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

public sealed record ClinicalHistoryResponse(
    Guid PatientId,
    string PatientName,
    string MedicalRecordNumber,
    IReadOnlyCollection<ConsultationResponse> Consultations);

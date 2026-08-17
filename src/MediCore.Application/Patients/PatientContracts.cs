using MediCore.Domain.Patients;

namespace MediCore.Application.Patients;

public sealed record CreatePatientRequest(
    string FirstName,
    string LastName,
    string Cedula,
    PatientType PatientType,
    Sex Sex,
    DateOnly? DateOfBirth,
    string? Email,
    string? Phone,
    string? Address,
    string? EmergencyContactName,
    string? EmergencyContactPhone);

public sealed record UpdatePatientRequest(
    string FirstName,
    string LastName,
    string Cedula,
    PatientType PatientType,
    Sex Sex,
    DateOnly? DateOfBirth,
    string? Email,
    string? Phone,
    string? Address,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    bool IsActive);

public sealed record PatientResponse(
    Guid Id,
    string MedicalRecordNumber,
    string FirstName,
    string LastName,
    string FullName,
    string Cedula,
    PatientType PatientType,
    Sex Sex,
    DateOnly? DateOfBirth,
    string? Email,
    string? Phone,
    string? Address,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

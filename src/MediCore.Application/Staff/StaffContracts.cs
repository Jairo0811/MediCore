using MediCore.Domain.Staff;

namespace MediCore.Application.Staff;

public sealed record CreateMedicalStaffRequest(
    string FirstName,
    string LastName,
    string Cedula,
    StaffType StaffType,
    string? Specialty,
    string? LicenseNumber,
    string? WorkShift,
    string? Email,
    string? Phone);

public sealed record UpdateMedicalStaffRequest(
    string FirstName,
    string LastName,
    string Cedula,
    StaffType StaffType,
    string? Specialty,
    string? LicenseNumber,
    string? WorkShift,
    string? Email,
    string? Phone,
    bool IsActive);

public sealed record MedicalStaffResponse(
    Guid Id,
    string EmployeeCode,
    string FirstName,
    string LastName,
    string FullName,
    string Cedula,
    StaffType StaffType,
    string? Specialty,
    string? LicenseNumber,
    string? WorkShift,
    string? Email,
    string? Phone,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

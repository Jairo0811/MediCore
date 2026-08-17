using MediCore.Domain.Common;

namespace MediCore.Domain.Staff;

public sealed class MedicalStaff : BaseEntity
{
    private MedicalStaff()
    {
    }

    public MedicalStaff(
        string firstName,
        string lastName,
        string cedula,
        string employeeCode,
        StaffType staffType,
        string? specialty,
        string? licenseNumber,
        string? workShift,
        string? email,
        string? phone)
    {
        FirstName = firstName;
        LastName = lastName;
        Cedula = cedula;
        EmployeeCode = employeeCode;
        StaffType = staffType;
        Specialty = specialty;
        LicenseNumber = licenseNumber;
        WorkShift = workShift;
        Email = email;
        Phone = phone;
    }

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Cedula { get; private set; } = string.Empty;
    public string EmployeeCode { get; private set; } = string.Empty;
    public StaffType StaffType { get; private set; }
    public string? Specialty { get; private set; }
    public string? LicenseNumber { get; private set; }
    public string? WorkShift { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public bool IsActive { get; private set; } = true;

    public string FullName => $"{FirstName} {LastName}".Trim();

    public void Update(
        string firstName,
        string lastName,
        string cedula,
        StaffType staffType,
        string? specialty,
        string? licenseNumber,
        string? workShift,
        string? email,
        string? phone)
    {
        FirstName = firstName;
        LastName = lastName;
        Cedula = cedula;
        StaffType = staffType;
        Specialty = specialty;
        LicenseNumber = licenseNumber;
        WorkShift = workShift;
        Email = email;
        Phone = phone;
        MarkAsUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    public void Reactivate()
    {
        IsActive = true;
        MarkAsUpdated();
    }
}

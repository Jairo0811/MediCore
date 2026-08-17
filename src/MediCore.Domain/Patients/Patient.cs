using MediCore.Domain.Common;

namespace MediCore.Domain.Patients;

public sealed class Patient : BaseEntity
{
    private Patient()
    {
    }

    public Patient(
        string firstName,
        string lastName,
        string cedula,
        string medicalRecordNumber,
        PatientType patientType,
        Sex sex,
        DateOnly? dateOfBirth,
        string? email,
        string? phone,
        string? address,
        string? emergencyContactName,
        string? emergencyContactPhone)
    {
        FirstName = firstName;
        LastName = lastName;
        Cedula = cedula;
        MedicalRecordNumber = medicalRecordNumber;
        PatientType = patientType;
        Sex = sex;
        DateOfBirth = dateOfBirth;
        Email = email;
        Phone = phone;
        Address = address;
        EmergencyContactName = emergencyContactName;
        EmergencyContactPhone = emergencyContactPhone;
    }

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Cedula { get; private set; } = string.Empty;
    public string MedicalRecordNumber { get; private set; } = string.Empty;
    public PatientType PatientType { get; private set; }
    public Sex Sex { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Address { get; private set; }
    public string? EmergencyContactName { get; private set; }
    public string? EmergencyContactPhone { get; private set; }
    public bool IsActive { get; private set; } = true;

    public string FullName => $"{FirstName} {LastName}".Trim();

    public void Update(
        string firstName,
        string lastName,
        string cedula,
        PatientType patientType,
        Sex sex,
        DateOnly? dateOfBirth,
        string? email,
        string? phone,
        string? address,
        string? emergencyContactName,
        string? emergencyContactPhone)
    {
        FirstName = firstName;
        LastName = lastName;
        Cedula = cedula;
        PatientType = patientType;
        Sex = sex;
        DateOfBirth = dateOfBirth;
        Email = email;
        Phone = phone;
        Address = address;
        EmergencyContactName = emergencyContactName;
        EmergencyContactPhone = emergencyContactPhone;
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

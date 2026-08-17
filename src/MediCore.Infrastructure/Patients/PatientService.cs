using MediCore.Application.Common;
using MediCore.Application.Patients;
using MediCore.Domain.Patients;
using MediCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediCore.Infrastructure.Patients;

public sealed class PatientService(
    MediCoreDbContext dbContext,
    ICedulaValidator cedulaValidator) : IPatientService
{
    public async Task<IReadOnlyCollection<PatientResponse>> GetAllAsync(
        string? search,
        bool includeInactive,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Patients.AsNoTracking();

        if (!includeInactive)
        {
            query = query.Where(patient => patient.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(patient =>
                patient.FirstName.Contains(term) ||
                patient.LastName.Contains(term) ||
                patient.Cedula.Contains(term) ||
                patient.MedicalRecordNumber.Contains(term));
        }

        return await query
            .OrderBy(patient => patient.LastName)
            .ThenBy(patient => patient.FirstName)
            .Select(patient => Map(patient))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<PatientResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var patient = await dbContext.Patients
            .AsNoTracking()
            .SingleOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

        return patient is null ? null : Map(patient);
    }

    public async Task<OperationResult<PatientResponse>> CreateAsync(
        CreatePatientRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await ValidateRequestAsync(request.Cedula, null, cancellationToken);
        if (!validation.Succeeded)
        {
            return OperationResult<PatientResponse>.Failure(
                validation.ErrorCode ?? "validation_error",
                validation.ErrorMessage ?? "Los datos del paciente no son válidos.");
        }

        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
        {
            return OperationResult<PatientResponse>.Failure(
                "name_required",
                "El nombre y apellido del paciente son obligatorios.");
        }

        var patient = new Patient(
            request.FirstName.Trim(),
            request.LastName.Trim(),
            cedulaValidator.Normalize(request.Cedula),
            $"MC-{Guid.NewGuid():N}"[..11].ToUpperInvariant(),
            request.PatientType,
            request.Sex,
            request.DateOfBirth,
            NormalizeOptional(request.Email),
            NormalizeOptional(request.Phone),
            NormalizeOptional(request.Address),
            NormalizeOptional(request.EmergencyContactName),
            NormalizeOptional(request.EmergencyContactPhone));

        dbContext.Patients.Add(patient);
        await dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<PatientResponse>.Success(Map(patient));
    }

    public async Task<OperationResult<PatientResponse>> UpdateAsync(
        Guid id,
        UpdatePatientRequest request,
        CancellationToken cancellationToken)
    {
        var patient = await dbContext.Patients
            .SingleOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

        if (patient is null)
        {
            return OperationResult<PatientResponse>.Failure("not_found", "Paciente no encontrado.");
        }

        var validation = await ValidateRequestAsync(request.Cedula, id, cancellationToken);
        if (!validation.Succeeded)
        {
            return OperationResult<PatientResponse>.Failure(
                validation.ErrorCode ?? "validation_error",
                validation.ErrorMessage ?? "Los datos del paciente no son válidos.");
        }

        patient.Update(
            request.FirstName.Trim(),
            request.LastName.Trim(),
            cedulaValidator.Normalize(request.Cedula),
            request.PatientType,
            request.Sex,
            request.DateOfBirth,
            NormalizeOptional(request.Email),
            NormalizeOptional(request.Phone),
            NormalizeOptional(request.Address),
            NormalizeOptional(request.EmergencyContactName),
            NormalizeOptional(request.EmergencyContactPhone));

        if (request.IsActive)
        {
            patient.Reactivate();
        }
        else
        {
            patient.Deactivate();
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<PatientResponse>.Success(Map(patient));
    }

    public async Task<OperationResult<bool>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
    {
        var patient = await dbContext.Patients
            .SingleOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

        if (patient is null)
        {
            return OperationResult<bool>.Failure("not_found", "Paciente no encontrado.");
        }

        patient.Deactivate();
        await dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<bool>.Success(true);
    }

    private async Task<OperationResult<bool>> ValidateRequestAsync(
        string cedula,
        Guid? currentPatientId,
        CancellationToken cancellationToken)
    {
        if (!cedulaValidator.IsValid(cedula))
        {
            return OperationResult<bool>.Failure(
                "invalid_cedula",
                "La cédula dominicana no supera la validación de formato y dígito de control.");
        }

        var normalized = cedulaValidator.Normalize(cedula);
        var cedulaInUse = await dbContext.Patients.AnyAsync(
            patient => patient.Cedula == normalized &&
                (!currentPatientId.HasValue || patient.Id != currentPatientId.Value),
            cancellationToken);

        return cedulaInUse
            ? OperationResult<bool>.Failure("cedula_in_use", "Ya existe un paciente con esa cédula.")
            : OperationResult<bool>.Success(true);
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static PatientResponse Map(Patient patient) =>
        new(
            patient.Id,
            patient.MedicalRecordNumber,
            patient.FirstName,
            patient.LastName,
            patient.FullName,
            patient.Cedula,
            patient.PatientType,
            patient.Sex,
            patient.DateOfBirth,
            patient.Email,
            patient.Phone,
            patient.Address,
            patient.EmergencyContactName,
            patient.EmergencyContactPhone,
            patient.IsActive,
            patient.CreatedAtUtc,
            patient.UpdatedAtUtc);
}

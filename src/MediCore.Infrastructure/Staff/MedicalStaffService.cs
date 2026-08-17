using MediCore.Application.Common;
using MediCore.Application.Staff;
using MediCore.Domain.Staff;
using MediCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediCore.Infrastructure.Staff;

public sealed class MedicalStaffService(
    MediCoreDbContext dbContext,
    ICedulaValidator cedulaValidator) : IMedicalStaffService
{
    public async Task<IReadOnlyCollection<MedicalStaffResponse>> GetAllAsync(
        string? search,
        bool includeInactive,
        CancellationToken cancellationToken)
    {
        var query = dbContext.MedicalStaff.AsNoTracking();

        if (!includeInactive)
        {
            query = query.Where(staff => staff.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(staff =>
                staff.FirstName.Contains(term) ||
                staff.LastName.Contains(term) ||
                staff.Cedula.Contains(term) ||
                staff.EmployeeCode.Contains(term) ||
                (staff.Specialty != null && staff.Specialty.Contains(term)));
        }

        return await query
            .OrderBy(staff => staff.LastName)
            .ThenBy(staff => staff.FirstName)
            .Select(staff => Map(staff))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<MedicalStaffResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var staff = await dbContext.MedicalStaff
            .AsNoTracking()
            .SingleOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

        return staff is null ? null : Map(staff);
    }

    public async Task<OperationResult<MedicalStaffResponse>> CreateAsync(
        CreateMedicalStaffRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await ValidateAsync(request.Cedula, null, cancellationToken);
        if (!validation.Succeeded)
        {
            return OperationResult<MedicalStaffResponse>.Failure(
                validation.ErrorCode ?? "validation_error",
                validation.ErrorMessage ?? "Los datos del personal no son válidos.");
        }

        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
        {
            return OperationResult<MedicalStaffResponse>.Failure(
                "name_required",
                "El nombre y apellido son obligatorios.");
        }

        var staff = new MedicalStaff(
            request.FirstName.Trim(),
            request.LastName.Trim(),
            cedulaValidator.Normalize(request.Cedula),
            $"EMP-{Guid.NewGuid():N}"[..12].ToUpperInvariant(),
            request.StaffType,
            NormalizeOptional(request.Specialty),
            NormalizeOptional(request.LicenseNumber),
            NormalizeOptional(request.WorkShift),
            NormalizeOptional(request.Email),
            NormalizeOptional(request.Phone));

        dbContext.MedicalStaff.Add(staff);
        await dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<MedicalStaffResponse>.Success(Map(staff));
    }

    public async Task<OperationResult<MedicalStaffResponse>> UpdateAsync(
        Guid id,
        UpdateMedicalStaffRequest request,
        CancellationToken cancellationToken)
    {
        var staff = await dbContext.MedicalStaff
            .SingleOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

        if (staff is null)
        {
            return OperationResult<MedicalStaffResponse>.Failure("not_found", "Personal no encontrado.");
        }

        var validation = await ValidateAsync(request.Cedula, id, cancellationToken);
        if (!validation.Succeeded)
        {
            return OperationResult<MedicalStaffResponse>.Failure(
                validation.ErrorCode ?? "validation_error",
                validation.ErrorMessage ?? "Los datos del personal no son válidos.");
        }

        staff.Update(
            request.FirstName.Trim(),
            request.LastName.Trim(),
            cedulaValidator.Normalize(request.Cedula),
            request.StaffType,
            NormalizeOptional(request.Specialty),
            NormalizeOptional(request.LicenseNumber),
            NormalizeOptional(request.WorkShift),
            NormalizeOptional(request.Email),
            NormalizeOptional(request.Phone));

        if (request.IsActive)
        {
            staff.Reactivate();
        }
        else
        {
            staff.Deactivate();
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<MedicalStaffResponse>.Success(Map(staff));
    }

    public async Task<OperationResult<bool>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
    {
        var staff = await dbContext.MedicalStaff
            .SingleOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

        if (staff is null)
        {
            return OperationResult<bool>.Failure("not_found", "Personal no encontrado.");
        }

        staff.Deactivate();
        await dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<bool>.Success(true);
    }

    private async Task<OperationResult<bool>> ValidateAsync(
        string cedula,
        Guid? currentStaffId,
        CancellationToken cancellationToken)
    {
        if (!cedulaValidator.IsValid(cedula))
        {
            return OperationResult<bool>.Failure(
                "invalid_cedula",
                "La cédula dominicana no supera la validación de formato y dígito de control.");
        }

        var normalized = cedulaValidator.Normalize(cedula);
        var inUse = await dbContext.MedicalStaff.AnyAsync(
            staff => staff.Cedula == normalized &&
                (!currentStaffId.HasValue || staff.Id != currentStaffId.Value),
            cancellationToken);

        return inUse
            ? OperationResult<bool>.Failure("cedula_in_use", "Ya existe un miembro del personal con esa cédula.")
            : OperationResult<bool>.Success(true);
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static MedicalStaffResponse Map(MedicalStaff staff) =>
        new(
            staff.Id,
            staff.EmployeeCode,
            staff.FirstName,
            staff.LastName,
            staff.FullName,
            staff.Cedula,
            staff.StaffType,
            staff.Specialty,
            staff.LicenseNumber,
            staff.WorkShift,
            staff.Email,
            staff.Phone,
            staff.IsActive,
            staff.CreatedAtUtc,
            staff.UpdatedAtUtc);
}

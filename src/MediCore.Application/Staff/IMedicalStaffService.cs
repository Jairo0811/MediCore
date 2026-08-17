using MediCore.Application.Common;

namespace MediCore.Application.Staff;

public interface IMedicalStaffService
{
    Task<IReadOnlyCollection<MedicalStaffResponse>> GetAllAsync(
        string? search,
        bool includeInactive,
        CancellationToken cancellationToken);

    Task<MedicalStaffResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<OperationResult<MedicalStaffResponse>> CreateAsync(
        CreateMedicalStaffRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<MedicalStaffResponse>> UpdateAsync(
        Guid id,
        UpdateMedicalStaffRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<bool>> DeactivateAsync(Guid id, CancellationToken cancellationToken);
}

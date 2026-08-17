using MediCore.Application.Common;

namespace MediCore.Application.Patients;

public interface IPatientService
{
    Task<IReadOnlyCollection<PatientResponse>> GetAllAsync(
        string? search,
        bool includeInactive,
        CancellationToken cancellationToken);

    Task<PatientResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<OperationResult<PatientResponse>> CreateAsync(
        CreatePatientRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<PatientResponse>> UpdateAsync(
        Guid id,
        UpdatePatientRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<bool>> DeactivateAsync(Guid id, CancellationToken cancellationToken);
}

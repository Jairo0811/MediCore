using MediCore.Application.Common;

namespace MediCore.Application.Consultations;

public interface IConsultationService
{
    Task<IReadOnlyCollection<ConsultationResponse>> GetAllAsync(
        Guid? patientId,
        Guid? medicalStaffId,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken);

    Task<ConsultationResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<ClinicalHistoryResponse?> GetClinicalHistoryAsync(
        Guid patientId,
        CancellationToken cancellationToken);

    Task<OperationResult<ConsultationResponse>> CreateAsync(
        CreateConsultationRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<ConsultationResponse>> UpdateAsync(
        Guid id,
        UpdateConsultationRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<ConsultationResponse>> ChangeStatusAsync(
        Guid id,
        ChangeConsultationStatusRequest request,
        CancellationToken cancellationToken);
}

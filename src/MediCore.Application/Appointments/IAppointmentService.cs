using MediCore.Application.Common;

namespace MediCore.Application.Appointments;

public interface IAppointmentService
{
    Task<IReadOnlyCollection<AppointmentResponse>> GetAllAsync(
        DateTime? fromUtc,
        DateTime? toUtc,
        Guid? patientId,
        Guid? medicalStaffId,
        CancellationToken cancellationToken);

    Task<AppointmentResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<OperationResult<AppointmentResponse>> CreateAsync(
        CreateAppointmentRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<AppointmentResponse>> UpdateAsync(
        Guid id,
        UpdateAppointmentRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<AppointmentResponse>> ChangeStatusAsync(
        Guid id,
        ChangeAppointmentStatusRequest request,
        CancellationToken cancellationToken);
}

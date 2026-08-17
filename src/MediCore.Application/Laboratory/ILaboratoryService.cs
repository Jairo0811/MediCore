using MediCore.Application.Common;

namespace MediCore.Application.Laboratory;

public interface ILaboratoryService
{
    Task<IReadOnlyCollection<LabTestDefinitionResponse>> GetDefinitionsAsync(bool includeInactive, CancellationToken cancellationToken);
    Task<OperationResult<LabTestDefinitionResponse>> CreateDefinitionAsync(CreateLabTestDefinitionRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<LabOrderResponse>> GetOrdersAsync(Guid? patientId, CancellationToken cancellationToken);
    Task<OperationResult<LabOrderResponse>> CreateOrderAsync(CreateLabOrderRequest request, CancellationToken cancellationToken);
    Task<OperationResult<LabOrderResponse>> RecordResultAsync(Guid itemId, RecordLabResultRequest request, string resultedBy, CancellationToken cancellationToken);
}

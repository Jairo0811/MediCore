using MediCore.Application.Common;

namespace MediCore.Application.Inventory;

public interface IInventoryService
{
    Task<IReadOnlyCollection<InventoryLotResponse>> GetLotsAsync(string? search, bool lowStockOnly, int? expiringWithinDays, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<InventoryMovementResponse>> GetKardexAsync(Guid lotId, CancellationToken cancellationToken);
    Task<OperationResult<InventoryLotResponse>> CreateLotAsync(CreateInventoryLotRequest request, string performedBy, CancellationToken cancellationToken);
    Task<OperationResult<InventoryLotResponse>> RecordMovementAsync(Guid lotId, CreateInventoryMovementRequest request, string performedBy, CancellationToken cancellationToken);
}

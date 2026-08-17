using MediCore.Domain.Inventory;

namespace MediCore.Application.Inventory;

public sealed record CreateInventoryLotRequest(Guid MedicationId, Guid StorageLocationId, string LotNumber, DateOnly ExpirationDate, decimal UnitCost, int InitialQuantity, int ReorderPoint);
public sealed record CreateInventoryMovementRequest(InventoryMovementType Type, int Quantity, string? Reference, string? Notes);
public sealed record InventoryLotResponse(Guid Id, Guid MedicationId, string MedicationCode, string MedicationName, Guid StorageLocationId, string StorageLocationName, string LotNumber, DateOnly ExpirationDate, decimal UnitCost, int QuantityOnHand, int ReorderPoint, bool IsLowStock, bool IsActive);
public sealed record InventoryMovementResponse(Guid Id, Guid InventoryLotId, InventoryMovementType Type, int QuantityDelta, int BalanceAfter, string PerformedBy, string? Reference, string? Notes, DateTime OccurredAtUtc);

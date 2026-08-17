using MediCore.Domain.Common;

namespace MediCore.Domain.Inventory;

public enum InventoryMovementType
{
    Receipt = 1,
    Dispense = 2,
    AdjustmentIncrease = 3,
    AdjustmentDecrease = 4
}

public sealed class InventoryMovement : BaseEntity
{
    private InventoryMovement() { }

    public InventoryMovement(Guid inventoryLotId, InventoryMovementType type, int quantityDelta, int balanceAfter, string performedBy, string? reference, string? notes)
    {
        InventoryLotId = inventoryLotId;
        Type = type;
        QuantityDelta = quantityDelta;
        BalanceAfter = balanceAfter;
        PerformedBy = performedBy;
        Reference = reference;
        Notes = notes;
        OccurredAtUtc = DateTime.UtcNow;
    }

    public Guid InventoryLotId { get; private set; }
    public InventoryMovementType Type { get; private set; }
    public int QuantityDelta { get; private set; }
    public int BalanceAfter { get; private set; }
    public string PerformedBy { get; private set; } = string.Empty;
    public string? Reference { get; private set; }
    public string? Notes { get; private set; }
    public DateTime OccurredAtUtc { get; private set; }
}

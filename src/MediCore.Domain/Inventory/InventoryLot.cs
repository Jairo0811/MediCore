using MediCore.Domain.Common;

namespace MediCore.Domain.Inventory;

public sealed class InventoryLot : BaseEntity
{
    private InventoryLot() { }

    public InventoryLot(Guid medicationId, Guid storageLocationId, string lotNumber, DateOnly expirationDate, decimal unitCost, int initialQuantity, int reorderPoint)
    {
        MedicationId = medicationId;
        StorageLocationId = storageLocationId;
        LotNumber = lotNumber;
        ExpirationDate = expirationDate;
        UnitCost = unitCost;
        QuantityOnHand = initialQuantity;
        ReorderPoint = reorderPoint;
    }

    public Guid MedicationId { get; private set; }
    public Guid StorageLocationId { get; private set; }
    public string LotNumber { get; private set; } = string.Empty;
    public DateOnly ExpirationDate { get; private set; }
    public decimal UnitCost { get; private set; }
    public int QuantityOnHand { get; private set; }
    public int ReorderPoint { get; private set; }
    public bool IsActive { get; private set; } = true;

    public bool IsLowStock => QuantityOnHand <= ReorderPoint;

    public void ApplyMovement(int quantityDelta)
    {
        if (QuantityOnHand + quantityDelta < 0) throw new InvalidOperationException("El movimiento dejaría el lote con existencias negativas.");
        QuantityOnHand += quantityDelta;
        MarkAsUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }
}

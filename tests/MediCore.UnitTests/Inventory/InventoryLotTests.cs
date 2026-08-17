using MediCore.Domain.Inventory;
using Xunit;

namespace MediCore.UnitTests.Inventory;

public sealed class InventoryLotTests
{
    [Fact]
    public void ApplyMovement_UpdatesBalance()
    {
        var lot = new InventoryLot(Guid.NewGuid(), Guid.NewGuid(), "LOT-001", DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6)), 10m, 20, 5);
        lot.ApplyMovement(-4);
        Assert.Equal(16, lot.QuantityOnHand);
        Assert.False(lot.IsLowStock);
    }

    [Fact]
    public void ApplyMovement_RejectsNegativeBalance()
    {
        var lot = new InventoryLot(Guid.NewGuid(), Guid.NewGuid(), "LOT-002", DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6)), 10m, 3, 1);
        Assert.Throws<InvalidOperationException>(() => lot.ApplyMovement(-4));
        Assert.Equal(3, lot.QuantityOnHand);
    }

    [Fact]
    public void IsLowStock_UsesReorderPoint()
    {
        var lot = new InventoryLot(Guid.NewGuid(), Guid.NewGuid(), "LOT-003", DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6)), 10m, 5, 5);
        Assert.True(lot.IsLowStock);
    }
}

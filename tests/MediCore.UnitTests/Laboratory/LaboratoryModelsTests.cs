using MediCore.Domain.Laboratory;
using Xunit;

namespace MediCore.UnitTests.Laboratory;

public sealed class LaboratoryModelsTests
{
    [Fact]
    public void LabOrder_TransitionsFromPendingToCompleted()
    {
        var order = new LabOrder(Guid.NewGuid(), Guid.NewGuid(), null, null);
        Assert.Equal(LabOrderStatus.Pending, order.Status);
        order.MarkInProgress();
        Assert.Equal(LabOrderStatus.InProgress, order.Status);
        order.Complete();
        Assert.Equal(LabOrderStatus.Completed, order.Status);
    }

    [Fact]
    public void LabOrderItem_SetResult_CapturesTraceability()
    {
        var item = new LabOrderItem(Guid.NewGuid(), Guid.NewGuid());
        item.SetResult("95", "Resultado validado", "laboratorio@medicore.local");
        Assert.Equal(LabResultStatus.Completed, item.Status);
        Assert.Equal("95", item.ResultValue);
        Assert.Equal("laboratorio@medicore.local", item.ResultedBy);
        Assert.NotNull(item.ResultedAtUtc);
    }
}

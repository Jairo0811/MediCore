using MediCore.Domain.Common;
using Xunit;

namespace MediCore.UnitTests.Common;

public sealed class BaseEntityTests
{
    [Fact]
    public void New_entity_has_identity_and_creation_timestamp()
    {
        var entity = new TestEntity();

        Assert.NotEqual(Guid.Empty, entity.Id);
        Assert.True(entity.CreatedAtUtc <= DateTime.UtcNow);
        Assert.Null(entity.UpdatedAtUtc);
    }

    private sealed class TestEntity : BaseEntity;
}

using MediCore.Domain.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediCore.Infrastructure.Persistence.Configurations;

public sealed class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
{
    public void Configure(EntityTypeBuilder<InventoryMovement> entity)
    {
        entity.Property(x => x.PerformedBy).HasMaxLength(180).IsRequired();
        entity.Property(x => x.Reference).HasMaxLength(160);
        entity.Property(x => x.Notes).HasMaxLength(1000);
        entity.HasIndex(x => new { x.InventoryLotId, x.OccurredAtUtc });
        entity.HasOne<InventoryLot>().WithMany().HasForeignKey(x => x.InventoryLotId).OnDelete(DeleteBehavior.Restrict);
    }
}

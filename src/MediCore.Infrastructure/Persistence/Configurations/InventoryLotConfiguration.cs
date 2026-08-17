using MediCore.Domain.Inventory;
using MediCore.Domain.Pharmacy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediCore.Infrastructure.Persistence.Configurations;

public sealed class InventoryLotConfiguration : IEntityTypeConfiguration<InventoryLot>
{
    public void Configure(EntityTypeBuilder<InventoryLot> entity)
    {
        entity.Property(x => x.LotNumber).HasMaxLength(80).IsRequired();
        entity.Property(x => x.UnitCost).HasPrecision(18, 4);
        entity.HasIndex(x => new { x.MedicationId, x.LotNumber }).IsUnique();
        entity.HasIndex(x => x.ExpirationDate);
        entity.HasOne<Medication>().WithMany().HasForeignKey(x => x.MedicationId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne<StorageLocation>().WithMany().HasForeignKey(x => x.StorageLocationId).OnDelete(DeleteBehavior.Restrict);
    }
}

using MediCore.Domain.Pharmacy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediCore.Infrastructure.Persistence.Configurations;

public sealed class StorageLocationConfiguration : IEntityTypeConfiguration<StorageLocation>
{
    public void Configure(EntityTypeBuilder<StorageLocation> builder)
    {
        builder.ToTable("StorageLocations");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Code).HasMaxLength(40).IsRequired();
        builder.Property(item => item.Name).HasMaxLength(160).IsRequired();
        builder.Property(item => item.Description).HasMaxLength(500);
        builder.HasIndex(item => item.Code).IsUnique();
        builder.HasIndex(item => item.Name);
        builder.HasIndex(item => item.IsActive);
    }
}

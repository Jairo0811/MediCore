using MediCore.Domain.Pharmacy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediCore.Infrastructure.Persistence.Configurations;

public sealed class PharmaceuticalBrandConfiguration : IEntityTypeConfiguration<PharmaceuticalBrand>
{
    public void Configure(EntityTypeBuilder<PharmaceuticalBrand> builder)
    {
        builder.ToTable("PharmaceuticalBrands");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Name).HasMaxLength(160).IsRequired();
        builder.Property(item => item.ManufacturerCountry).HasMaxLength(100);
        builder.Property(item => item.Website).HasMaxLength(300);
        builder.HasIndex(item => item.Name).IsUnique();
        builder.HasIndex(item => item.IsActive);
    }
}

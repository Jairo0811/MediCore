using MediCore.Domain.Pharmacy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediCore.Infrastructure.Persistence.Configurations;

public sealed class MedicationConfiguration : IEntityTypeConfiguration<Medication>
{
    public void Configure(EntityTypeBuilder<Medication> builder)
    {
        builder.ToTable("Medications");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Code).HasMaxLength(40).IsRequired();
        builder.Property(item => item.Name).HasMaxLength(180).IsRequired();
        builder.Property(item => item.GenericName).HasMaxLength(180);
        builder.Property(item => item.ActiveIngredient).HasMaxLength(240);
        builder.Property(item => item.Strength).HasMaxLength(80);
        builder.Property(item => item.DosageForm).HasMaxLength(100);
        builder.Property(item => item.UnitOfMeasure).HasMaxLength(60);
        builder.Property(item => item.Notes).HasMaxLength(1200);
        builder.HasIndex(item => item.Code).IsUnique();
        builder.HasIndex(item => item.Name);
        builder.HasIndex(item => item.GenericName);
        builder.HasIndex(item => item.DrugTypeId);
        builder.HasIndex(item => item.IsActive);
        builder.HasOne<DrugType>().WithMany().HasForeignKey(item => item.DrugTypeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<PharmaceuticalBrand>().WithMany().HasForeignKey(item => item.PharmaceuticalBrandId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<StorageLocation>().WithMany().HasForeignKey(item => item.StorageLocationId).OnDelete(DeleteBehavior.Restrict);
    }
}

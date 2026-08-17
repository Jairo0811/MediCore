using MediCore.Domain.Pharmacy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediCore.Infrastructure.Persistence.Configurations;

public sealed class DrugTypeConfiguration : IEntityTypeConfiguration<DrugType>
{
    public void Configure(EntityTypeBuilder<DrugType> builder)
    {
        builder.ToTable("DrugTypes");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Name).HasMaxLength(120).IsRequired();
        builder.Property(item => item.Description).HasMaxLength(500);
        builder.HasIndex(item => item.Name).IsUnique();
        builder.HasIndex(item => item.IsActive);
    }
}

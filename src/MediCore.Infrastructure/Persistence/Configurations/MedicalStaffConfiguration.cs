using MediCore.Domain.Staff;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediCore.Infrastructure.Persistence.Configurations;

public sealed class MedicalStaffConfiguration : IEntityTypeConfiguration<MedicalStaff>
{
    public void Configure(EntityTypeBuilder<MedicalStaff> builder)
    {
        builder.ToTable("MedicalStaff");
        builder.HasKey(staff => staff.Id);

        builder.Property(staff => staff.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(staff => staff.LastName).HasMaxLength(100).IsRequired();
        builder.Property(staff => staff.Cedula).HasMaxLength(11).IsRequired();
        builder.Property(staff => staff.EmployeeCode).HasMaxLength(24).IsRequired();
        builder.Property(staff => staff.Specialty).HasMaxLength(120);
        builder.Property(staff => staff.LicenseNumber).HasMaxLength(80);
        builder.Property(staff => staff.WorkShift).HasMaxLength(80);
        builder.Property(staff => staff.Email).HasMaxLength(180);
        builder.Property(staff => staff.Phone).HasMaxLength(30);
        builder.Ignore(staff => staff.FullName);

        builder.HasIndex(staff => staff.Cedula).IsUnique();
        builder.HasIndex(staff => staff.EmployeeCode).IsUnique();
        builder.HasIndex(staff => new { staff.LastName, staff.FirstName });
    }
}

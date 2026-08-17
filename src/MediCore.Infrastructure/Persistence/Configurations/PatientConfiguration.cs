using MediCore.Domain.Patients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediCore.Infrastructure.Persistence.Configurations;

public sealed class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");
        builder.HasKey(patient => patient.Id);

        builder.Property(patient => patient.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(patient => patient.LastName).HasMaxLength(100).IsRequired();
        builder.Property(patient => patient.Cedula).HasMaxLength(11).IsRequired();
        builder.Property(patient => patient.MedicalRecordNumber).HasMaxLength(32).IsRequired();
        builder.Property(patient => patient.Email).HasMaxLength(180);
        builder.Property(patient => patient.Phone).HasMaxLength(30);
        builder.Property(patient => patient.Address).HasMaxLength(300);
        builder.Property(patient => patient.EmergencyContactName).HasMaxLength(160);
        builder.Property(patient => patient.EmergencyContactPhone).HasMaxLength(30);
        builder.Ignore(patient => patient.FullName);

        builder.HasIndex(patient => patient.Cedula).IsUnique();
        builder.HasIndex(patient => patient.MedicalRecordNumber).IsUnique();
        builder.HasIndex(patient => new { patient.LastName, patient.FirstName });
    }
}

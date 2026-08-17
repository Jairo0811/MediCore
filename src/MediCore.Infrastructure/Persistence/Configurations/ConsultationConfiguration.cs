using MediCore.Domain.Consultations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediCore.Infrastructure.Persistence.Configurations;

public sealed class ConsultationConfiguration : IEntityTypeConfiguration<Consultation>
{
    public void Configure(EntityTypeBuilder<Consultation> builder)
    {
        builder.ToTable("Consultations");
        builder.HasKey(consultation => consultation.Id);

        builder.Property(consultation => consultation.Reason).HasMaxLength(500).IsRequired();
        builder.Property(consultation => consultation.Symptoms).HasMaxLength(2000);
        builder.Property(consultation => consultation.Diagnosis).HasMaxLength(2000);
        builder.Property(consultation => consultation.Recommendations).HasMaxLength(2000);
        builder.Property(consultation => consultation.Notes).HasMaxLength(3000);
        builder.Property(consultation => consultation.BloodPressure).HasMaxLength(20);
        builder.Property(consultation => consultation.TemperatureCelsius).HasPrecision(5, 2);
        builder.Property(consultation => consultation.WeightKg).HasPrecision(7, 2);
        builder.Property(consultation => consultation.HeightCm).HasPrecision(6, 2);

        builder.HasOne(consultation => consultation.Patient)
            .WithMany()
            .HasForeignKey(consultation => consultation.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(consultation => consultation.MedicalStaff)
            .WithMany()
            .HasForeignKey(consultation => consultation.MedicalStaffId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(consultation => consultation.Appointment)
            .WithMany()
            .HasForeignKey(consultation => consultation.AppointmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(consultation => new { consultation.PatientId, consultation.ConsultationDateUtc });
        builder.HasIndex(consultation => new { consultation.MedicalStaffId, consultation.ConsultationDateUtc });
        builder.HasIndex(consultation => consultation.AppointmentId);
    }
}

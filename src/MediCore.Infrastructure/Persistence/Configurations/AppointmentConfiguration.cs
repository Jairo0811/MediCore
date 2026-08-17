using MediCore.Domain.Appointments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediCore.Infrastructure.Persistence.Configurations;

public sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");
        builder.HasKey(appointment => appointment.Id);

        builder.Property(appointment => appointment.Reason).HasMaxLength(300).IsRequired();
        builder.Property(appointment => appointment.Notes).HasMaxLength(1500);
        builder.Property(appointment => appointment.CancellationReason).HasMaxLength(500);

        builder.HasOne(appointment => appointment.Patient)
            .WithMany()
            .HasForeignKey(appointment => appointment.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(appointment => appointment.MedicalStaff)
            .WithMany()
            .HasForeignKey(appointment => appointment.MedicalStaffId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(appointment => appointment.ScheduledStartUtc);
        builder.HasIndex(appointment => new { appointment.MedicalStaffId, appointment.ScheduledStartUtc });
        builder.HasIndex(appointment => new { appointment.PatientId, appointment.ScheduledStartUtc });
    }
}

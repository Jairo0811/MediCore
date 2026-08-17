using MediCore.Domain.Consultations;
using MediCore.Domain.Laboratory;
using MediCore.Domain.Patients;
using MediCore.Domain.Staff;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediCore.Infrastructure.Persistence.Configurations;

public sealed class LabTestDefinitionConfiguration : IEntityTypeConfiguration<LabTestDefinition>
{
    public void Configure(EntityTypeBuilder<LabTestDefinition> entity)
    {
        entity.Property(x => x.Code).HasMaxLength(40).IsRequired(); entity.Property(x => x.Name).HasMaxLength(160).IsRequired();
        entity.Property(x => x.SampleType).HasMaxLength(100); entity.Property(x => x.Unit).HasMaxLength(60); entity.Property(x => x.ReferenceRange).HasMaxLength(250);
        entity.HasIndex(x => x.Code).IsUnique(); entity.HasIndex(x => x.Name);
    }
}
public sealed class LabOrderConfiguration : IEntityTypeConfiguration<LabOrder>
{
    public void Configure(EntityTypeBuilder<LabOrder> entity)
    {
        entity.Property(x => x.ClinicalNotes).HasMaxLength(1500); entity.HasIndex(x => new { x.PatientId, x.OrderedAtUtc });
        entity.HasOne<Patient>().WithMany().HasForeignKey(x => x.PatientId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne<MedicalStaff>().WithMany().HasForeignKey(x => x.RequestedByMedicalStaffId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne<Consultation>().WithMany().HasForeignKey(x => x.ConsultationId).OnDelete(DeleteBehavior.Restrict);
    }
}
public sealed class LabOrderItemConfiguration : IEntityTypeConfiguration<LabOrderItem>
{
    public void Configure(EntityTypeBuilder<LabOrderItem> entity)
    {
        entity.Property(x => x.ResultValue).HasMaxLength(250); entity.Property(x => x.ResultText).HasMaxLength(1500); entity.Property(x => x.ResultedBy).HasMaxLength(180);
        entity.HasIndex(x => new { x.LabOrderId, x.LabTestDefinitionId }).IsUnique();
        entity.HasOne<LabOrder>().WithMany().HasForeignKey(x => x.LabOrderId).OnDelete(DeleteBehavior.Cascade);
        entity.HasOne<LabTestDefinition>().WithMany().HasForeignKey(x => x.LabTestDefinitionId).OnDelete(DeleteBehavior.Restrict);
    }
}

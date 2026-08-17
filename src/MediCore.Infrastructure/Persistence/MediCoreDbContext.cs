using MediCore.Domain.Appointments;
using MediCore.Domain.Audit;
using MediCore.Domain.Consultations;
using MediCore.Domain.Patients;
using MediCore.Domain.Staff;
using MediCore.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MediCore.Infrastructure.Persistence;

public sealed class MediCoreDbContext(DbContextOptions<MediCoreDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<MedicalStaff> MedicalStaff => Set<MedicalStaff>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Consultation> Consultations => Set<Consultation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MediCoreDbContext).Assembly);

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(user => user.FullName).HasMaxLength(160).IsRequired();
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(token => token.Id);
            entity.Property(token => token.TokenHash).HasMaxLength(64).IsRequired();
            entity.HasIndex(token => token.TokenHash).IsUnique();
            entity.HasOne(token => token.User)
                .WithMany()
                .HasForeignKey(token => token.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.Property(log => log.Action).HasMaxLength(100).IsRequired();
            entity.Property(log => log.EntityName).HasMaxLength(120).IsRequired();
            entity.Property(log => log.EntityId).HasMaxLength(80);
            entity.Property(log => log.IpAddress).HasMaxLength(64);
            entity.Property(log => log.Details).HasMaxLength(2000);
            entity.HasIndex(log => log.CreatedAtUtc);
        });
    }
}

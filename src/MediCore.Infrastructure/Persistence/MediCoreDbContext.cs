using Microsoft.EntityFrameworkCore;

namespace MediCore.Infrastructure.Persistence;

public sealed class MediCoreDbContext(DbContextOptions<MediCoreDbContext> options)
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MediCoreDbContext).Assembly);
    }
}

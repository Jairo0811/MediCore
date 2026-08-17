using MediCore.Application.Identity;
using MediCore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MediCore.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task InitializeMediCoreDatabaseAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MediCoreDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
        await SeedRolesAsync(scope.ServiceProvider, cancellationToken);
    }

    public static async Task SeedMediCoreRolesAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        await SeedRolesAsync(scope.ServiceProvider, cancellationToken);
    }

    private static async Task SeedRolesAsync(
        IServiceProvider services,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        foreach (var role in RoleNames.All)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (await roleManager.RoleExistsAsync(role)) continue;

            var result = await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            if (!result.Succeeded)
            {
                var details = string.Join("; ", result.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"No se pudo crear el rol {role}: {details}");
            }
        }
    }
}

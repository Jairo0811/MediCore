using MediCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediCore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is required.");
        }

        services.AddDbContext<MediCoreDbContext>(options =>
            options.UseSqlServer(connectionString));

        services
            .AddHealthChecks()
            .AddDbContextCheck<MediCoreDbContext>(
                name: "database",
                tags: ["ready"]);

        return services;
    }
}

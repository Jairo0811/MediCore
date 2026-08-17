using System.Text.Json;
using MediCore.Api.Endpoints;
using MediCore.Infrastructure;
using MediCore.Infrastructure.Identity;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("MediCoreWeb", policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins);
        }

        policy.AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (builder.Configuration.GetValue<bool>("Database:InitializeOnStartup"))
{
    await app.Services.InitializeMediCoreDatabaseAsync();
}

app.UseExceptionHandler();
app.UseCors("MediCoreWeb");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api", () => Results.Ok(new
{
    name = "MediCore.Api",
    slogan = "La gestión médica en un solo lugar.",
    version = "0.6.0-phases1-5"
}));

app.MapAuthEndpoints();
app.MapPatientEndpoints();

app.MapHealthChecks("/api/health/live", new HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = WriteHealthResponse
});

app.MapHealthChecks("/api/health/ready", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("ready"),
    ResponseWriter = WriteHealthResponse
});

app.MapHealthChecks("/api/health", new HealthCheckOptions
{
    ResponseWriter = WriteHealthResponse
});

app.Run();

static Task WriteHealthResponse(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json";

    return context.Response.WriteAsync(JsonSerializer.Serialize(new
    {
        status = report.Status.ToString(),
        timestampUtc = DateTime.UtcNow,
        checks = report.Entries.Select(entry => new
        {
            name = entry.Key,
            status = entry.Value.Status.ToString(),
            description = entry.Value.Description,
            durationMs = entry.Value.Duration.TotalMilliseconds
        })
    }));
}

public partial class Program;

using System.Text.Json;
using MediCore.Api.Endpoints;
using MediCore.Api.Middleware;
using MediCore.Infrastructure;
using MediCore.Infrastructure.Identity;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddRateLimiter(options => options.AddFixedWindowLimiter("auth", limiter =>
{
    limiter.PermitLimit = 10;
    limiter.Window = TimeSpan.FromMinutes(1);
    limiter.QueueLimit = 0;
    limiter.AutoReplenishment = true;
}));
builder.Services.AddCors(options => options.AddPolicy("MediCoreWeb", policy =>
{
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
    if (allowedOrigins.Length > 0) policy.WithOrigins(allowedOrigins);
    policy.AllowAnyHeader().AllowAnyMethod();
}));

var app = builder.Build();
if (app.Environment.IsDevelopment()) app.MapOpenApi();

if (app.Environment.IsProduction())
{
    var signingKey = builder.Configuration["Jwt:SigningKey"];
    if (builder.Configuration.GetValue<bool>("Auth:AllowBootstrapAdmin"))
        throw new InvalidOperationException("Auth:AllowBootstrapAdmin must be false in Production.");
    if (string.IsNullOrWhiteSpace(signingKey) || signingKey.Length < 32 || signingKey.Contains("ChangeThis", StringComparison.OrdinalIgnoreCase) || signingKey.Contains("Development-Key", StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("A production-grade JWT signing key with at least 32 characters is required.");
}

var initializeDatabase = builder.Configuration.GetValue<bool>("Database:InitializeOnStartup");
if (initializeDatabase)
{
    if (app.Environment.IsProduction())
        throw new InvalidOperationException("Automatic database migration is disabled in Production. Apply the reviewed migration before starting the application.");
    await app.Services.InitializeMediCoreDatabaseAsync();
}
else if (app.Environment.IsProduction())
{
    // The schema must already have been migrated by the deployment pipeline.
    // Seeding fixed RBAC roles is idempotent and does not alter the schema.
    await app.Services.SeedMediCoreRolesAsync();
}

app.UseExceptionHandler();
app.UseMiddleware<RequestObservabilityMiddleware>();
app.UseCors("MediCoreWeb");
app.UseRateLimiter();
app.UseAuthentication();
app.UseMiddleware<AuditTrailMiddleware>();
app.UseAuthorization();

app.MapGet("/api", () => Results.Ok(new { name = "MediCore.Api", slogan = "La gestión médica en un solo lugar.", version = "1.0.0" }));
app.MapAuthEndpoints();
app.MapPatientEndpoints();
app.MapMedicalStaffEndpoints();
app.MapAppointmentEndpoints();
app.MapConsultationEndpoints();
app.MapPharmacyEndpoints();
app.MapInventoryEndpoints();
app.MapLaboratoryEndpoints();
app.MapAnalyticsEndpoints();
app.MapAuditEndpoints();

app.MapHealthChecks("/api/health/live", new HealthCheckOptions { Predicate = _ => false, ResponseWriter = WriteHealthResponse });
app.MapHealthChecks("/api/health/ready", new HealthCheckOptions { Predicate = registration => registration.Tags.Contains("ready"), ResponseWriter = WriteHealthResponse });
app.MapHealthChecks("/api/health", new HealthCheckOptions { ResponseWriter = WriteHealthResponse });
app.Run();

static Task WriteHealthResponse(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json";
    return context.Response.WriteAsync(JsonSerializer.Serialize(new
    {
        status = report.Status.ToString(),
        timestampUtc = DateTime.UtcNow,
        correlationId = context.TraceIdentifier,
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

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
    limiter.PermitLimit = 10; limiter.Window = TimeSpan.FromMinutes(1); limiter.QueueLimit = 0; limiter.AutoReplenishment = true;
}));
builder.Services.AddCors(options => options.AddPolicy("MediCoreWeb", policy =>
{
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
    if (allowedOrigins.Length > 0) policy.WithOrigins(allowedOrigins);
    policy.AllowAnyHeader().AllowAnyMethod();
}));

var app = builder.Build();
if (app.Environment.IsDevelopment()) app.MapOpenApi();
if (app.Environment.IsProduction() && builder.Configuration.GetValue<bool>("Auth:AllowBootstrapAdmin")) throw new InvalidOperationException("Auth:AllowBootstrapAdmin must be false in Production.");
if (app.Environment.IsProduction() && builder.Configuration["Jwt:SigningKey"]?.Contains("ChangeThis", StringComparison.OrdinalIgnoreCase) == true) throw new InvalidOperationException("A production-grade JWT signing key is required.");
if (builder.Configuration.GetValue<bool>("Database:InitializeOnStartup"))
{
    if (app.Environment.IsProduction()) throw new InvalidOperationException("Automatic EnsureCreated initialization is disabled in Production. Provision the schema through the controlled deployment process.");
    await app.Services.InitializeMediCoreDatabaseAsync();
}

app.UseExceptionHandler(); app.UseMiddleware<RequestObservabilityMiddleware>(); app.UseCors("MediCoreWeb"); app.UseRateLimiter(); app.UseAuthentication(); app.UseAuthorization();
app.MapGet("/api", () => Results.Ok(new { name = "MediCore.Api", slogan = "La gestión médica en un solo lugar.", version = "1.0.0" }));
app.MapAuthEndpoints(); app.MapPatientEndpoints(); app.MapMedicalStaffEndpoints(); app.MapAppointmentEndpoints(); app.MapConsultationEndpoints(); app.MapPharmacyEndpoints(); app.MapInventoryEndpoints(); app.MapLaboratoryEndpoints(); app.MapAnalyticsEndpoints();
app.MapHealthChecks("/api/health/live", new HealthCheckOptions { Predicate = _ => false, ResponseWriter = WriteHealthResponse });
app.MapHealthChecks("/api/health/ready", new HealthCheckOptions { Predicate = registration => registration.Tags.Contains("ready"), ResponseWriter = WriteHealthResponse });
app.MapHealthChecks("/api/health", new HealthCheckOptions { ResponseWriter = WriteHealthResponse });
app.Run();

static Task WriteHealthResponse(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json";
    return context.Response.WriteAsync(JsonSerializer.Serialize(new { status = report.Status.ToString(), timestampUtc = DateTime.UtcNow, correlationId = context.TraceIdentifier, checks = report.Entries.Select(entry => new { name = entry.Key, status = entry.Value.Status.ToString(), description = entry.Value.Description, durationMs = entry.Value.Duration.TotalMilliseconds }) }));
}

public partial class Program;

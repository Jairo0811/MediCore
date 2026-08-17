var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("MediCoreWeb", policy =>
    {
        policy
            .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [])
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("MediCoreWeb");

app.MapGet("/api/health", () => Results.Ok(new
{
    service = "MediCore.Api",
    status = "healthy",
    timestampUtc = DateTime.UtcNow
}));

app.Run();

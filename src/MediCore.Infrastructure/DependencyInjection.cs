using System.Text;
using MediCore.Application.Appointments;
using MediCore.Application.Common;
using MediCore.Application.Consultations;
using MediCore.Application.Identity;
using MediCore.Application.Patients;
using MediCore.Application.Pharmacy;
using MediCore.Application.Staff;
using MediCore.Infrastructure.Appointments;
using MediCore.Infrastructure.Consultations;
using MediCore.Infrastructure.Identity;
using MediCore.Infrastructure.Patients;
using MediCore.Infrastructure.Persistence;
using MediCore.Infrastructure.Pharmacy;
using MediCore.Infrastructure.Staff;
using MediCore.Infrastructure.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace MediCore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString)) throw new InvalidOperationException("Connection string 'DefaultConnection' is required.");

        services.AddDbContext<MediCoreDbContext>(options => options.UseSqlServer(connectionString));
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequiredLength = 10;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.User.RequireUniqueEmail = true;
        }).AddRoles<IdentityRole<Guid>>().AddEntityFrameworkStores<MediCoreDbContext>().AddDefaultTokenProviders();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
        var signingKey = string.IsNullOrWhiteSpace(jwtOptions.SigningKey) ? "MediCore-Development-Key-Change-Before-Production-2026" : jwtOptions.SigningKey;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                ClockSkew = TimeSpan.FromSeconds(30)
            };
        });

        services.AddAuthorization();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICedulaValidator, DominicanCedulaValidator>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IMedicalStaffService, MedicalStaffService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IConsultationService, ConsultationService>();
        services.AddScoped<IPharmacyService, PharmacyService>();
        services.AddHealthChecks().AddDbContextCheck<MediCoreDbContext>(name: "database", tags: ["ready"]);
        return services;
    }
}

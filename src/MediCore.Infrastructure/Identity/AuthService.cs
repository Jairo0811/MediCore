using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MediCore.Application.Common;
using MediCore.Application.Identity;
using MediCore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MediCore.Infrastructure.Identity;

public sealed class AuthService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    MediCoreDbContext dbContext,
    IOptions<JwtOptions> jwtOptions,
    IConfiguration configuration) : IAuthService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<OperationResult<AuthResponse>> BootstrapAdminAsync(
        BootstrapAdminRequest request,
        CancellationToken cancellationToken)
    {
        var allowBootstrap = configuration.GetValue<bool>("Auth:AllowBootstrapAdmin");
        if (!allowBootstrap)
        {
            return OperationResult<AuthResponse>.Failure(
                "bootstrap_disabled",
                "El registro inicial de administrador está deshabilitado.");
        }

        if (await userManager.Users.AnyAsync(cancellationToken))
        {
            return OperationResult<AuthResponse>.Failure(
                "bootstrap_completed",
                "MediCore ya tiene usuarios registrados.");
        }

        var userResult = await CreateUserAsync(
            request.Email,
            request.Password,
            request.FullName,
            RoleNames.Administrator);

        if (!userResult.Succeeded || userResult.Value is null)
        {
            return OperationResult<AuthResponse>.Failure(
                userResult.ErrorCode ?? "registration_failed",
                userResult.ErrorMessage ?? "No fue posible crear el administrador.");
        }

        return OperationResult<AuthResponse>.Success(
            await IssueTokensAsync(userResult.Value, cancellationToken));
    }

    public async Task<OperationResult<AuthResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim();
        var user = await userManager.FindByEmailAsync(normalizedEmail);

        if (user is null || !user.IsActive || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return OperationResult<AuthResponse>.Failure(
                "invalid_credentials",
                "Correo electrónico o contraseña incorrectos.");
        }

        return OperationResult<AuthResponse>.Success(
            await IssueTokensAsync(user, cancellationToken));
    }

    public async Task<OperationResult<AuthResponse>> RefreshAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var tokenHash = HashToken(request.RefreshToken);
        var storedToken = await dbContext.RefreshTokens
            .Include(token => token.User)
            .SingleOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);

        if (storedToken is null || !storedToken.IsActive || !storedToken.User.IsActive)
        {
            return OperationResult<AuthResponse>.Failure(
                "invalid_refresh_token",
                "El refresh token no es válido o ya expiró.");
        }

        storedToken.RevokedAtUtc = DateTime.UtcNow;
        var response = await IssueTokensAsync(storedToken.User, cancellationToken);
        storedToken.ReplacedByTokenHash = HashToken(response.RefreshToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return OperationResult<AuthResponse>.Success(response);
    }

    public async Task<OperationResult<bool>> LogoutAsync(
        LogoutRequest request,
        CancellationToken cancellationToken)
    {
        var tokenHash = HashToken(request.RefreshToken);
        var storedToken = await dbContext.RefreshTokens
            .SingleOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);

        if (storedToken is null)
        {
            return OperationResult<bool>.Success(true);
        }

        if (storedToken.RevokedAtUtc is null)
        {
            storedToken.RevokedAtUtc = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return OperationResult<bool>.Success(true);
    }

    public async Task<OperationResult<CurrentUserResponse>> RegisterAsync(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        if (!RoleNames.All.Contains(request.Role, StringComparer.Ordinal))
        {
            return OperationResult<CurrentUserResponse>.Failure(
                "invalid_role",
                "El rol solicitado no existe en MediCore.");
        }

        var result = await CreateUserAsync(
            request.Email,
            request.Password,
            request.FullName,
            request.Role);

        if (!result.Succeeded || result.Value is null)
        {
            return OperationResult<CurrentUserResponse>.Failure(
                result.ErrorCode ?? "registration_failed",
                result.ErrorMessage ?? "No fue posible crear el usuario.");
        }

        return OperationResult<CurrentUserResponse>.Success(
            await MapUserAsync(result.Value));
    }

    public async Task<CurrentUserResponse?> GetCurrentUserAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .SingleOrDefaultAsync(candidate => candidate.Id == userId, cancellationToken);

        return user is null ? null : await MapUserAsync(user);
    }

    private async Task<OperationResult<ApplicationUser>> CreateUserAsync(
        string email,
        string password,
        string fullName,
        string role)
    {
        var normalizedEmail = email.Trim();
        if (await userManager.FindByEmailAsync(normalizedEmail) is not null)
        {
            return OperationResult<ApplicationUser>.Failure(
                "email_in_use",
                "Ya existe una cuenta con ese correo electrónico.");
        }

        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }

        var user = new ApplicationUser
        {
            UserName = normalizedEmail,
            Email = normalizedEmail,
            FullName = fullName.Trim(),
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            var message = string.Join(" ", createResult.Errors.Select(error => error.Description));
            return OperationResult<ApplicationUser>.Failure("identity_error", message);
        }

        var roleResult = await userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);
            var message = string.Join(" ", roleResult.Errors.Select(error => error.Description));
            return OperationResult<ApplicationUser>.Failure("role_assignment_failed", message);
        }

        return OperationResult<ApplicationUser>.Success(user);
    }

    private async Task<AuthResponse> IssueTokensAsync(
        ApplicationUser user,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_jwtOptions.SigningKey) || _jwtOptions.SigningKey.Length < 32)
        {
            throw new InvalidOperationException(
                "Jwt:SigningKey debe contener al menos 32 caracteres.");
        }

        var roles = await userManager.GetRolesAsync(user);
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        var rawRefreshToken = GenerateRefreshToken();

        dbContext.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = HashToken(rawRefreshToken),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays)
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponse(
            accessToken,
            rawRefreshToken,
            expiresAtUtc,
            new CurrentUserResponse(
                user.Id,
                user.Email ?? string.Empty,
                user.FullName,
                roles.ToArray()));
    }

    private async Task<CurrentUserResponse> MapUserAsync(ApplicationUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        return new CurrentUserResponse(
            user.Id,
            user.Email ?? string.Empty,
            user.FullName,
            roles.ToArray());
    }

    private static string GenerateRefreshToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    private static string HashToken(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}

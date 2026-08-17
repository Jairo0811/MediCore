using System.Security.Claims;
using MediCore.Application.Common;
using MediCore.Application.Identity;
using Microsoft.AspNetCore.RateLimiting;

namespace MediCore.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/auth").WithTags("Identity");

        group.MapPost("/bootstrap-admin", BootstrapAdminAsync).AllowAnonymous().RequireRateLimiting("auth");
        group.MapPost("/login", LoginAsync).AllowAnonymous().RequireRateLimiting("auth");
        group.MapPost("/refresh", RefreshAsync).AllowAnonymous().RequireRateLimiting("auth");
        group.MapPost("/logout", LogoutAsync).AllowAnonymous().RequireRateLimiting("auth");
        group.MapPost("/users", RegisterAsync).RequireAuthorization(policy => policy.RequireRole(RoleNames.Administrator));
        group.MapGet("/me", GetMeAsync).RequireAuthorization();
        return endpoints;
    }

    private static async Task<IResult> BootstrapAdminAsync(BootstrapAdminRequest request, IAuthService authService, CancellationToken cancellationToken) => ToResult(await authService.BootstrapAdminAsync(request, cancellationToken));
    private static async Task<IResult> LoginAsync(LoginRequest request, IAuthService authService, CancellationToken cancellationToken) => ToResult(await authService.LoginAsync(request, cancellationToken));
    private static async Task<IResult> RefreshAsync(RefreshTokenRequest request, IAuthService authService, CancellationToken cancellationToken) => ToResult(await authService.RefreshAsync(request, cancellationToken));
    private static async Task<IResult> LogoutAsync(LogoutRequest request, IAuthService authService, CancellationToken cancellationToken) => ToResult(await authService.LogoutAsync(request, cancellationToken));
    private static async Task<IResult> RegisterAsync(RegisterUserRequest request, IAuthService authService, CancellationToken cancellationToken) => ToResult(await authService.RegisterAsync(request, cancellationToken));
    private static async Task<IResult> GetMeAsync(ClaimsPrincipal principal, IAuthService authService, CancellationToken cancellationToken)
    {
        var idValue = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(idValue, out var userId)) return Results.Unauthorized();
        var user = await authService.GetCurrentUserAsync(userId, cancellationToken);
        return user is null ? Results.NotFound() : Results.Ok(user);
    }
    private static IResult ToResult<T>(OperationResult<T> result)
    {
        if (result.Succeeded) return Results.Ok(result.Value);
        return result.ErrorCode switch
        {
            "invalid_credentials" or "invalid_refresh_token" => Results.Unauthorized(),
            "email_in_use" or "bootstrap_completed" => Results.Conflict(new { error = result.ErrorCode, message = result.ErrorMessage }),
            _ => Results.BadRequest(new { error = result.ErrorCode, message = result.ErrorMessage })
        };
    }
}

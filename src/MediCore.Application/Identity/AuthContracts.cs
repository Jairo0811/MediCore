namespace MediCore.Application.Identity;

public sealed record LoginRequest(string Email, string Password);

public sealed record RefreshTokenRequest(string RefreshToken);

public sealed record LogoutRequest(string RefreshToken);

public sealed record RegisterUserRequest(
    string Email,
    string Password,
    string FullName,
    string Role);

public sealed record BootstrapAdminRequest(
    string Email,
    string Password,
    string FullName);

public sealed record CurrentUserResponse(
    Guid Id,
    string Email,
    string FullName,
    IReadOnlyCollection<string> Roles);

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc,
    CurrentUserResponse User);

using MediCore.Application.Common;

namespace MediCore.Application.Identity;

public interface IAuthService
{
    Task<OperationResult<AuthResponse>> BootstrapAdminAsync(
        BootstrapAdminRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<AuthResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<AuthResponse>> RefreshAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<bool>> LogoutAsync(
        LogoutRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<CurrentUserResponse>> RegisterAsync(
        RegisterUserRequest request,
        CancellationToken cancellationToken);

    Task<CurrentUserResponse?> GetCurrentUserAsync(
        Guid userId,
        CancellationToken cancellationToken);
}

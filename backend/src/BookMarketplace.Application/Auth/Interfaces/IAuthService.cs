using BookMarketplace.Application.Auth.DTOs;

namespace BookMarketplace.Application.Auth.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(
        RegisterRequest request,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task<AuthResult> LoginAsync(
        LoginRequest request,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    // Validates the refresh token, revokes it and issues a new pair.
    Task<AuthResult> RefreshAsync(
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    Task<UserResponse> GetCurrentUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
using BookMarketplace.Domain.Entities;

namespace BookMarketplace.Application.Auth.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAtUtc) CreateAccessToken(User user);

    // Returns a new random token. This raw value goes to the client only.
    string GenerateRefreshToken();

    // Deterministic hash used to store and look up refresh tokens.
    string HashToken(string token);
}
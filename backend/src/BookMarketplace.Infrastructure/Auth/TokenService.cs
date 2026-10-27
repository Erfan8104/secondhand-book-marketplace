using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BookMarketplace.Application.Auth.Interfaces;
using BookMarketplace.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace BookMarketplace.Infrastructure.Auth;

public sealed class TokenService(
    IOptions<JwtOptions> options,
    TimeProvider timeProvider) : ITokenService
{
    private readonly JwtOptions _options = options.Value;

    private readonly JsonWebTokenHandler _handler = new();

    private readonly SigningCredentials _signingCredentials = new(
        new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(options.Value.SecretKey)),
        SecurityAlgorithms.HmacSha256);

    public (string Token, DateTime ExpiresAtUtc) CreateAccessToken(User user)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var expiresAtUtc = now.AddMinutes(_options.AccessTokenMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            IssuedAt = now,
            NotBefore = now,
            Expires = expiresAtUtc,
            SigningCredentials = _signingCredentials,
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("role", user.Role.ToString())
            })
        };

        return (_handler.CreateToken(descriptor), expiresAtUtc);
    }

    public string GenerateRefreshToken() =>
        System.Buffers.Text.Base64Url.EncodeToString(
            RandomNumberGenerator.GetBytes(64));

    public string HashToken(string token) =>
        Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
namespace BookMarketplace.Application.Auth.DTOs;

public sealed record AuthResult(
    UserResponse User,
    AuthTokens Tokens);
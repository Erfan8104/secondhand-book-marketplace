using BookMarketplace.Application.Auth.DTOs;
using BookMarketplace.Application.Auth.Interfaces;
using BookMarketplace.Application.Common;
using BookMarketplace.Application.Common.Exceptions;
using BookMarketplace.Domain.Entities;
using BookMarketplace.Domain.Enums;
using BookMarketplace.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

namespace BookMarketplace.Infrastructure.Auth;

public sealed class AuthService(
    MarketplaceDbContext db,
    IPasswordHasher<User> passwordHasher,
    ITokenService tokenService,
    IOptions<JwtOptions> jwtOptions,
    TimeProvider timeProvider) : IAuthService
{
    private const int MaxFailedLoginAttempts = 5;

    private static readonly TimeSpan LockoutDuration =
        TimeSpan.FromMinutes(15);

    private const string InvalidCredentialsMessage =
        "Invalid phone number or password.";

    private const string InvalidRefreshTokenMessage =
        "Invalid or expired refresh token.";

    // Used to keep response time similar when the phone number is unknown.
    private static readonly User DummyUser = new();

    private static readonly string DummyHash =
        new PasswordHasher<User>().HashPassword(
            DummyUser,
            "timing-equalizer-password");

    public async Task<AuthResult> RegisterAsync(
        RegisterRequest request,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        if (!PhoneNumberNormalizer.TryNormalize(
                request.PhoneNumber,
                out var phoneNumber))
        {
            throw new ArgumentException(
                "Phone number is not a valid Iranian mobile number.");
        }

        var email = NormalizeEmail(request.Email);

        if (await db.Users.AnyAsync(
                u => u.PhoneNumber == phoneNumber,
                cancellationToken))
        {
            throw new ConflictException(
                "A user with this phone number already exists.");
        }

        if (email is not null && await db.Users.AnyAsync(
                u => u.Email == email,
                cancellationToken))
        {
            throw new ConflictException(
                "A user with this email already exists.");
        }

        var user = new User
        {
            FullName = request.FullName.Trim(),
            PhoneNumber = phoneNumber,
            Email = email,
            Role = UserRole.Customer
        };

        user.PasswordHash = passwordHasher.HashPassword(
            user,
            request.Password);

        db.Users.Add(user);

        var tokens = CreateSession(user, ipAddress);

        try
        {
            // User and refresh token are saved in one transaction.
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation
            })
        {
            // Two simultaneous requests passed the checks above.
            // The unique index in the database caught the second one.
            throw new ConflictException(
                "A user with this phone number or email already exists.");
        }

        return new AuthResult(UserResponse.From(user), tokens);
    }

    public async Task<AuthResult> LoginAsync(
        LoginRequest request,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        if (!PhoneNumberNormalizer.TryNormalize(
                request.PhoneNumber,
                out var phoneNumber))
        {
            throw new AuthenticationFailedException(
                InvalidCredentialsMessage);
        }

        var now = timeProvider.GetUtcNow().UtcDateTime;

        var user = await db.Users.FirstOrDefaultAsync(
            u => u.PhoneNumber == phoneNumber,
            cancellationToken);

        if (user is null)
        {
            passwordHasher.VerifyHashedPassword(
                DummyUser,
                DummyHash,
                request.Password);

            throw new AuthenticationFailedException(
                InvalidCredentialsMessage);
        }

        if (user.LockoutEndUtc is { } lockoutEnd && lockoutEnd > now)
        {
            throw new AccountLockedException(lockoutEnd);
        }

        var verification = passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (verification == PasswordVerificationResult.Failed)
        {
            user.FailedLoginCount++;

            if (user.FailedLoginCount >= MaxFailedLoginAttempts)
            {
                user.LockoutEndUtc = now.Add(LockoutDuration);
                user.FailedLoginCount = 0;
            }

            await db.SaveChangesAsync(cancellationToken);

            throw new AuthenticationFailedException(
                InvalidCredentialsMessage);
        }

        if (!user.IsActive)
        {
            throw new AuthenticationFailedException(
                InvalidCredentialsMessage);
        }

        user.FailedLoginCount = 0;
        user.LockoutEndUtc = null;

        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            // The stored hash uses an older algorithm or fewer iterations.
            user.PasswordHash = passwordHasher.HashPassword(
                user,
                request.Password);
        }

        var tokens = CreateSession(user, ipAddress);

        await db.SaveChangesAsync(cancellationToken);

        return new AuthResult(UserResponse.From(user), tokens);
    }

    public async Task<AuthResult> RefreshAsync(
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new AuthenticationFailedException(
                InvalidRefreshTokenMessage);
        }

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var hash = tokenService.HashToken(refreshToken);

        var stored = await db.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == hash, cancellationToken);

        if (stored is null)
        {
            throw new AuthenticationFailedException(
                InvalidRefreshTokenMessage);
        }

        if (stored.IsRevoked())
        {
            // A rotated or logged-out token was presented again.
            // It may have been stolen, so end every session of this user.
            await RevokeAllActiveTokensAsync(
                stored.UserId,
                now,
                cancellationToken);

            throw new AuthenticationFailedException(
                InvalidRefreshTokenMessage);
        }

        if (stored.IsExpired(now) || !stored.User.IsActive)
        {
            throw new AuthenticationFailedException(
                InvalidRefreshTokenMessage);
        }

        var tokens = CreateSession(stored.User, ipAddress);

        stored.RevokedAtUtc = now;
        stored.ReplacedByTokenHash = tokenService.HashToken(
            tokens.RefreshToken);

        await db.SaveChangesAsync(cancellationToken);

        return new AuthResult(UserResponse.From(stored.User), tokens);
    }

    public async Task LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return;
        }

        var hash = tokenService.HashToken(refreshToken);

        var stored = await db.RefreshTokens.FirstOrDefaultAsync(
            t => t.TokenHash == hash,
            cancellationToken);

        if (stored is null || stored.IsRevoked())
        {
            return;
        }

        stored.RevokedAtUtc = timeProvider.GetUtcNow().UtcDateTime;

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserResponse> GetCurrentUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                u => u.Id == userId && u.IsActive,
                cancellationToken);

        if (user is null)
        {
            throw new AuthenticationFailedException(
                "User not found or inactive.");
        }

        return UserResponse.From(user);
    }

    // Builds the access token and a new refresh token entity.
    // It does NOT call SaveChanges: the caller saves everything at once.
    private AuthTokens CreateSession(User user, string? ipAddress)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;

        var (accessToken, accessExpiresAtUtc) =
            tokenService.CreateAccessToken(user);

        var rawRefreshToken = tokenService.GenerateRefreshToken();
        var refreshExpiresAtUtc = now.AddDays(
            jwtOptions.Value.RefreshTokenDays);

        db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = tokenService.HashToken(rawRefreshToken),
            ExpiresAtUtc = refreshExpiresAtUtc,
            CreatedByIp = ipAddress
        });

        return new AuthTokens(
            accessToken,
            accessExpiresAtUtc,
            rawRefreshToken,
            refreshExpiresAtUtc);
    }

    private async Task RevokeAllActiveTokensAsync(
        Guid userId,
        DateTime now,
        CancellationToken cancellationToken)
    {
        await db.RefreshTokens
            .Where(t =>
                t.UserId == userId &&
                t.RevokedAtUtc == null &&
                t.ExpiresAtUtc > now)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    t => t.RevokedAtUtc,
                    (DateTime?)now),
                cancellationToken);
    }

    private static string? NormalizeEmail(string? email) =>
        string.IsNullOrWhiteSpace(email)
            ? null
            : email.Trim().ToLowerInvariant();
}
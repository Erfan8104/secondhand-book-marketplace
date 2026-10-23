namespace BookMarketplace.Application.Common.Exceptions;

public sealed class AccountLockedException(DateTime lockoutEndUtc)
    : Exception("Too many failed login attempts. Please try again later.")
{
    public DateTime LockoutEndUtc { get; } = lockoutEndUtc;
}
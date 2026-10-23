using BookMarketplace.Domain.Entities;

namespace BookMarketplace.Application.Auth.DTOs;

public sealed record UserResponse(
    Guid Id,
    string FullName,
    string PhoneNumber,
    string? Email,
    string Role)
{
    public static UserResponse From(User user) => new(
        user.Id,
        user.FullName,
        user.PhoneNumber,
        user.Email,
        user.Role.ToString());
}
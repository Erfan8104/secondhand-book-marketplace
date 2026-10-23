using System.ComponentModel.DataAnnotations;
using BookMarketplace.Application.Common.Validation;

namespace BookMarketplace.Application.Auth.DTOs;

public sealed class RegisterRequest
{
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(150, MinimumLength = 2)]
    public string FullName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [StringLength(30)]
    [IranianMobile]
    public string PhoneNumber { get; init; } = string.Empty;

    [EmailAddress]
    [StringLength(254)]
    public string? Email { get; init; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(
        128,
        MinimumLength = 8,
        ErrorMessage = "Password must be between 8 and 128 characters.")]
    public string Password { get; init; } = string.Empty;
}
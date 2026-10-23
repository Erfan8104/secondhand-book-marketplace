using System.ComponentModel.DataAnnotations;
using BookMarketplace.Application.Common.Validation;

namespace BookMarketplace.Application.Auth.DTOs;

public sealed class LoginRequest
{
    [Required(ErrorMessage = "Phone number is required.")]
    [StringLength(30)]
    [IranianMobile]
    public string PhoneNumber { get; init; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(128)]
    public string Password { get; init; } = string.Empty;
}
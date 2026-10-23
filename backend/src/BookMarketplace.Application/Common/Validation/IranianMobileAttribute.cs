    using System.ComponentModel.DataAnnotations;

namespace BookMarketplace.Application.Common.Validation;

[AttributeUsage(
    AttributeTargets.Property |
    AttributeTargets.Field |
    AttributeTargets.Parameter)]
public sealed class IranianMobileAttribute() : ValidationAttribute(
    "Phone number must be a valid Iranian mobile number, e.g. 09121234567.")
{
    public override bool IsValid(object? value) =>
        value is string text &&
        PhoneNumberNormalizer.TryNormalize(text, out _);
}
using System.Text;

namespace BookMarketplace.Application.Common;

public static class PhoneNumberNormalizer
{
    // Converts Iranian mobile numbers to E.164, e.g. +989121234567.
    public static bool TryNormalize(string? input, out string normalized)
    {
        normalized = string.Empty;

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var text = input.Trim();
        var builder = new StringBuilder();
        for (var i = 0; i < text.Length; i++)
        {
        
          var ch = text[i];

            if (ch is >= '0' and <= '9')
            {
                builder.Append(ch);
            }
            else if (ch is >= '\u06F0' and <= '\u06F9')
            {
                // Persian digits ۰-۹
                builder.Append((char)('0' + (ch - '\u06F0')));
            }
            else if (ch is >= '\u0660' and <= '\u0669')
            {
                // Arabic-Indic digits ٠-٩
                builder.Append((char)('0' + (ch - '\u0660')));
            }
            else if (ch == '+' && i == 0)
            {
                continue;
            }
            else if (ch is ' ' or '-' or '(' or ')')
            {
                continue;
            }
            else
            {
                return false;
            }
        }

        var digits = builder.ToString();

        if (digits.Length == 14 && digits.StartsWith("0098"))
        {
            digits = digits[4..];
        }
        else if (digits.Length == 12 && digits.StartsWith("98"))
        {
            digits = digits[2..];
        }
        else if (digits.Length == 11 && digits.StartsWith('0'))
        {
            digits = digits[1..];
        }

        // An Iranian mobile number is 10 digits and starts with 9.
        if (digits.Length != 10 || digits[0] != '9')
        {
            return false;
        }

        normalized = "+98" + digits;
        return true;
    }
}
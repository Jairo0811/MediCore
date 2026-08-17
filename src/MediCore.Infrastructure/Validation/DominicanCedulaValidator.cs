using System.Security.Cryptography;
using System.Text;
using MediCore.Application.Common;
using Microsoft.Extensions.Configuration;

namespace MediCore.Infrastructure.Validation;

public sealed class DominicanCedulaValidator(IConfiguration configuration) : ICedulaValidator
{
    public bool IsValid(string cedula)
    {
        var normalized = Normalize(cedula);
        if (normalized.Length != 11 || normalized.Any(character => !char.IsDigit(character)))
        {
            return false;
        }

        if (PassesLuhn(normalized))
        {
            return true;
        }

        var configuredHashes = configuration
            .GetSection("CedulaValidation:LuhnExceptionHashes")
            .Get<string[]>() ?? [];

        if (configuredHashes.Length == 0)
        {
            return false;
        }

        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(normalized)));
        return configuredHashes.Any(candidate =>
            string.Equals(candidate.Trim(), hash, StringComparison.OrdinalIgnoreCase));
    }

    public string Normalize(string cedula) =>
        new(cedula.Where(char.IsDigit).ToArray());

    private static bool PassesLuhn(string cedula)
    {
        var digits = cedula.Select(character => character - '0').Reverse().ToArray();
        var checkDigit = digits[0];
        var sum = checkDigit;

        for (var index = 1; index < digits.Length; index++)
        {
            var value = digits[index];
            if (index % 2 == 1)
            {
                value *= 2;
                if (value > 9)
                {
                    value -= 9;
                }
            }

            sum += value;
        }

        return sum % 10 == 0;
    }
}

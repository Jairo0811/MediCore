using MediCore.Infrastructure.Validation;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace MediCore.UnitTests.Validation;

public sealed class DominicanCedulaValidatorTests
{
    private static DominicanCedulaValidator CreateValidator(params string[] exceptionHashes)
    {
        var values = new Dictionary<string, string?>();
        for (var index = 0; index < exceptionHashes.Length; index++)
        {
            values[$"CedulaValidation:LuhnExceptionHashes:{index}"] = exceptionHashes[index];
        }

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();

        return new DominicanCedulaValidator(configuration);
    }

    [Fact]
    public void IsValid_ReturnsTrue_WhenCedulaPassesLuhn()
    {
        var validator = CreateValidator();

        Assert.True(validator.IsValid("100-0000000-9"));
    }

    [Fact]
    public void IsValid_ReturnsFalse_WhenCedulaFailsLuhn()
    {
        var validator = CreateValidator();

        Assert.False(validator.IsValid("100-0000000-8"));
    }

    [Fact]
    public void IsValid_ReturnsFalse_WhenInputDoesNotHaveElevenDigits()
    {
        var validator = CreateValidator();

        Assert.False(validator.IsValid("123456789"));
    }
}

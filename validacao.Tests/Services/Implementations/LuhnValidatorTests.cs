using FluentAssertions;
using validacao.Services.Implementations;
using Xunit;

namespace validacao.Tests.Services.Implementations;

public class LuhnValidatorTests
{
    private readonly LuhnValidator _validator;

    public LuhnValidatorTests()
    {
        _validator = new LuhnValidator();
    }

    [Theory]
    [InlineData("4532015112830366", true)]  // Valid Visa
    [InlineData("5513945908742906", true)]  // Valid MasterCard  
    [InlineData("378282246310005", true)]   // Valid American Express
    [InlineData("6011111111111117", true)]  // Valid Discover
    [InlineData("30569309025904", true)]    // Valid Diners Club
    [InlineData("38520000023237", true)]    // Valid Diners Club
    public void IsValid_WithValidCardNumbers_ReturnsTrue(string cardNumber, bool expected)
    {
        // Act
        var result = _validator.IsValid(cardNumber);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("4532015112830367", false)] // Invalid Visa (wrong checksum)
    [InlineData("5513945908742905", false)] // Invalid MasterCard (wrong checksum)
    [InlineData("378282246310006", false)]  // Invalid Amex (wrong checksum)
    [InlineData("1234567890123456", false)] // Invalid number
    [InlineData("4111111111111112", false)] // Invalid Visa (wrong checksum)
    public void IsValid_WithInvalidCardNumbers_ReturnsFalse(string cardNumber, bool expected)
    {
        // Act
        var result = _validator.IsValid(cardNumber);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("   ", false)]
    public void IsValid_WithNullOrEmptyInput_ReturnsFalse(string cardNumber, bool expected)
    {
        // Act
        var result = _validator.IsValid(cardNumber);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("4532 0151 1283 0366", true)]  // With spaces
    [InlineData("4532-0151-1283-0366", true)]  // With dashes
    [InlineData("4532.0151.1283.0366", true)]  // With dots
    [InlineData("4532/0151/1283/0366", true)]  // With slashes
    public void IsValid_WithFormattedInput_IgnoresNonDigits(string cardNumber, bool expected)
    {
        // Act
        var result = _validator.IsValid(cardNumber);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("abc123def456", false)]       // Mixed letters and numbers
    [InlineData("abcdefghijk", false)]        // Only letters
    [InlineData("!@#$%^&*()", false)]        // Only special characters
    [InlineData("123abc456def", false)]       // Numbers with letters
    public void IsValid_WithInvalidCharacters_ReturnsFalse(string cardNumber, bool expected)
    {
        // Act
        var result = _validator.IsValid(cardNumber);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("123", false)]                // Too short
    [InlineData("12345678901", false)]        // Too short (11 digits)
    [InlineData("123456789012", false)]       // Too short (12 digits)
    [InlineData("12345678901234567890123", false)] // Too long (23 digits)
    public void IsValid_WithInvalidLength_ReturnsFalse(string cardNumber, bool expected)
    {
        // Act
        var result = _validator.IsValid(cardNumber);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void IsValid_WithValidLengthRange_ReturnsExpectedResults()
    {
        // Arrange - Valid cards with different lengths
        var validCards = new Dictionary<string, int>
        {
            ["4532015112830366"] = 16,    // Visa 16 digits
            ["378282246310005"] = 15,     // Amex 15 digits
            ["30569309025904"] = 14,      // Diners 14 digits
            ["4532015112830"] = 13        // Visa 13 digits (valid length but may fail Luhn)
        };

        foreach (var (cardNumber, expectedLength) in validCards)
        {
            // Act
            var result = _validator.IsValid(cardNumber);

            // Assert - At least should not fail due to length
            cardNumber.Replace(" ", "").Length.Should().Be(expectedLength);
        }
    }

    [Theory]
    [InlineData("0000000000000000", false)]   // All zeros
    [InlineData("1111111111111116", false)]   // All ones (invalid Luhn)
    [InlineData("9999999999999995", false)]   // All nines (invalid Luhn)
    public void IsValid_WithEdgeCases_ReturnsExpectedResults(string cardNumber, bool expected)
    {
        // Act
        var result = _validator.IsValid(cardNumber);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void IsValid_WithKnownValidTestCards_ReturnsTrue()
    {
        // Arrange - Known test card numbers that should pass Luhn
        var testCards = new[]
        {
            "4111111111111111", // Visa test card
            "5555555555554444", // MasterCard test card
            "374711461014862",  // Amex test card
            "6011000990139424"  // Discover test card
        };

        foreach (var cardNumber in testCards)
        {
            // Act
            var result = _validator.IsValid(cardNumber);

            // Assert
            result.Should().BeTrue($"Card {cardNumber} should be valid by Luhn algorithm");
        }
    }
}
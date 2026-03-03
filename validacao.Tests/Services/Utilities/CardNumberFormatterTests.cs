using FluentAssertions;
using validacao.Services.Utilities;
using Xunit;

namespace validacao.Tests.Services.Utilities;

public class CardNumberFormatterTests
{
    [Theory]
    [InlineData("4532015112830366", "4532015112830366")]
    [InlineData("4532 0151 1283 0366", "4532015112830366")]
    [InlineData("4532-0151-1283-0366", "4532015112830366")]
    [InlineData("4532.0151.1283.0366", "4532015112830366")]
    [InlineData("4532/0151/1283/0366", "4532015112830366")]
    public void Clean_WithVariousFormats_ReturnsOnlyDigits(string input, string expected)
    {
        // Act
        var result = CardNumberFormatter.Clean(input);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(null, "")]
    [InlineData("   ", "")]
    [InlineData("\t\n", "")]
    public void Clean_WithNullOrWhitespace_ReturnsEmptyString(string input, string expected)
    {
        // Act
        var result = CardNumberFormatter.Clean(input);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("abc123def456", "123456")]
    [InlineData("!@#123$%^456&*()", "123456")]
    [InlineData("a1b2c3d4e5f6", "123456")]
    [InlineData("4532abc0151def1283ghi0366", "4532015112830366")]
    public void Clean_WithMixedCharacters_ReturnsOnlyDigits(string input, string expected)
    {
        // Act
        var result = CardNumberFormatter.Clean(input);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("abcdef", "")]
    [InlineData("!@#$%^&*()", "")]
    [InlineData("", "")]
    [InlineData("   ", "")]
    public void Clean_WithNoDigits_ReturnsEmptyString(string input, string expected)
    {
        // Act
        var result = CardNumberFormatter.Clean(input);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("4532015112830366", true)]   // 16 digits
    [InlineData("378282246310005", true)]    // 15 digits  
    [InlineData("30569309025904", true)]     // 14 digits
    [InlineData("4532015112830", true)]      // 13 digits
    [InlineData("4532015112830123456", true)] // 19 digits
    public void IsValidLength_WithValidLengths_ReturnsTrue(string cardNumber, bool expected)
    {
        // Act
        var result = CardNumberFormatter.IsValidLength(cardNumber);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("123456789012", false)]      // 12 digits (too short)
    [InlineData("123456789", false)]         // 9 digits (too short)
    [InlineData("12345678901234567890", false)] // 20 digits (too long)
    [InlineData("123456789012345678901234", false)] // 24 digits (too long)
    [InlineData("", false)]                  // Empty
    [InlineData(null, false)]               // Null
    public void IsValidLength_WithInvalidLengths_ReturnsFalse(string cardNumber, bool expected)
    {
        // Act
        var result = CardNumberFormatter.IsValidLength(cardNumber);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("4532015112830366", "************0366")] // Default 4 digits
    [InlineData("4532015112830366", "***********30366")] // 5 digits visible
    [InlineData("4532015112830366", "**********830366")] // 6 digits visible
    [InlineData("378282246310005", "***********0005")]   // 15-digit card
    [InlineData("30569309025904", "**********5904")]     // 14-digit card
    public void Mask_WithDifferentVisibleDigits_ReturnsCorrectMask(string cardNumber, string expected)
    {
        // Arrange
        var visibleDigits = expected.Length - expected.Count(c => c == '*');

        // Act
        var result = CardNumberFormatter.Mask(cardNumber, visibleDigits);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("1234", "1234")]     // Shorter than visible digits
    [InlineData("12", "12")]         // Much shorter
    [InlineData("", "")]             // Empty
    [InlineData(null, null)]         // Null
    public void Mask_WithShortInputs_ReturnsOriginalValue(string cardNumber, string expected)
    {
        // Act
        var result = CardNumberFormatter.Mask(cardNumber, 4);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Mask_WithZeroVisibleDigits_MasksEntireNumber()
    {
        // Arrange
        var cardNumber = "4532015112830366";
        var expected = "****************";

        // Act
        var result = CardNumberFormatter.Mask(cardNumber, 0);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("4532015112830366", "4532 0151 1283 0366")]
    [InlineData("378282246310005", "3782 8224 6310 005")]
    [InlineData("30569309025904", "3056 9309 0259 04")]
    [InlineData("123456789", "1234 5678 9")]
    public void Format_WithValidCardNumbers_ReturnsFormattedString(string cardNumber, string expected)
    {
        // Act
        var result = CardNumberFormatter.Format(cardNumber);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(null, "")]
    [InlineData("   ", "")]
    public void Format_WithInvalidInput_ReturnsEmptyString(string cardNumber, string expected)
    {
        // Act
        var result = CardNumberFormatter.Format(cardNumber);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("123", "123")]       // Less than 4 characters
    [InlineData("12", "12")]         // 2 characters
    [InlineData("1", "1")]           // 1 character
    public void Format_WithShortInputs_ReturnsUnchanged(string cardNumber, string expected)
    {
        // Act
        var result = CardNumberFormatter.Format(cardNumber);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Clean_And_Format_IntegrationTest()
    {
        // Arrange
        var input = "4532-0151-1283-0366";
        var expectedClean = "4532015112830366";
        var expectedFormatted = "4532 0151 1283 0366";

        // Act
        var cleaned = CardNumberFormatter.Clean(input);
        var formatted = CardNumberFormatter.Format(cleaned);

        // Assert
        cleaned.Should().Be(expectedClean);
        formatted.Should().Be(expectedFormatted);
    }

    [Fact]
    public void Clean_And_Mask_IntegrationTest()
    {
        // Arrange
        var input = "4532 0151 1283 0366";
        var expectedCleaned = "4532015112830366";
        var expectedMasked = "************0366";

        // Act
        var cleaned = CardNumberFormatter.Clean(input);
        var masked = CardNumberFormatter.Mask(cleaned);

        // Assert
        cleaned.Should().Be(expectedCleaned);
        masked.Should().Be(expectedMasked);
    }

    [Theory]
    [InlineData("4532015112830366", 3, "*************366")] // 3 visible
    [InlineData("4532015112830366", 16, "4532015112830366")]  // More visible than total length
    [InlineData("4532015112830366", 20, "4532015112830366")]  // Much more visible than total length
    public void Mask_WithEdgeCases_HandlesCorrectly(string cardNumber, int visibleDigits, string expected)
    {
        // Act
        var result = CardNumberFormatter.Mask(cardNumber, visibleDigits);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void AllMethods_WithSameInput_ProduceConsistentResults()
    {
        // Arrange
        var input = "4532-0151-1283-0366";

        // Act
        var cleaned = CardNumberFormatter.Clean(input);
        var isValidLength = CardNumberFormatter.IsValidLength(cleaned);
        var masked = CardNumberFormatter.Mask(cleaned);
        var formatted = CardNumberFormatter.Format(cleaned);

        // Assert
        cleaned.Should().Be("4532015112830366");
        isValidLength.Should().BeTrue();
        masked.Should().Be("************0366");
        formatted.Should().Be("4532 0151 1283 0366");
    }
}
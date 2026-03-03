using FluentAssertions;
using validacao.Services.Implementations;
using Xunit;

namespace validacao.Tests.Services.Implementations;

public class CardBrandServiceTests
{
    private readonly CardBrandService _service;

    public CardBrandServiceTests()
    {
        _service = new CardBrandService();
    }

    [Theory]
    [InlineData("4111111111111111", "Visa")]
    [InlineData("4532015112830366", "Visa")]
    [InlineData("4000000000000002", "Visa")]
    [InlineData("4999999999999991", "Visa")]
    public void Identify_WithVisaCards_ReturnsVisa(string cardNumber, string expectedBrand)
    {
        // Act
        var result = _service.Identify(cardNumber);

        // Assert
        result.Should().Be(expectedBrand);
    }

    [Theory]
    [InlineData("5555555555554444", "MasterCard")]
    [InlineData("5513945908742906", "MasterCard")]
    [InlineData("5105105105105100", "MasterCard")]
    [InlineData("5199999999999991", "MasterCard")]
    [InlineData("2221000000000009", "MasterCard")] // New range 2221-2720
    [InlineData("2720999999999996", "MasterCard")] // New range 2221-2720
    public void Identify_WithMasterCardCards_ReturnsMasterCard(string cardNumber, string expectedBrand)
    {
        // Act
        var result = _service.Identify(cardNumber);

        // Assert
        result.Should().Be(expectedBrand);
    }

    [Theory]
    [InlineData("378282246310005", "American Express")]
    [InlineData("371449635398431", "American Express")]
    [InlineData("340000000000009", "American Express")]
    [InlineData("370000000000002", "American Express")]
    public void Identify_WithAmexCards_ReturnsAmericanExpress(string cardNumber, string expectedBrand)
    {
        // Act
        var result = _service.Identify(cardNumber);

        // Assert
        result.Should().Be(expectedBrand);
    }

    [Theory]
    [InlineData("6011111111111117", "Discover")]
    [InlineData("6011000990139424", "Discover")]
    [InlineData("6500000000000002", "Discover")]
    [InlineData("6440000000000004", "Discover")] // 644-649 range
    [InlineData("6490000000000001", "Discover")] // 644-649 range
    public void Identify_WithDiscoverCards_ReturnsDiscover(string cardNumber, string expectedBrand)
    {
        // Act
        var result = _service.Identify(cardNumber);

        // Assert
        result.Should().Be(expectedBrand);
    }

    [Theory]
    [InlineData("6062820000000005", "Hipercard")]
    [InlineData("6062821234567890", "Hipercard")]
    public void Identify_WithHipercardCards_ReturnsHipercard(string cardNumber, string expectedBrand)
    {
        // Act
        var result = _service.Identify(cardNumber);

        // Assert
        result.Should().Be(expectedBrand);
    }

    [Theory]
    [InlineData("5041111111111111", "Elo")]  // Elo prefix 5041 (não conflita com Visa)
    [InlineData("5067111111111111", "Elo")]  // Elo prefix 5067 (não conflita com Visa)
    [InlineData("5090111111111111", "Elo")]  // Elo prefix 5090 (não conflita com Visa)
    [InlineData("6277111111111111", "Elo")]  // Elo prefix 6277 (não conflita)
    [InlineData("6362111111111111", "Elo")]  // Elo prefix 6362 (não conflita)
    [InlineData("6363111111111111", "Elo")]  // Elo prefix 6363 (não conflita)
    public void Identify_WithEloCards_ReturnsElo(string cardNumber, string expectedBrand)
    {
        // Act
        var result = _service.Identify(cardNumber);

        // Assert
        result.Should().Be(expectedBrand);
    }

    [Theory]
    [InlineData("1234567890123456", "Unknown")]
    [InlineData("9999999999999999", "Unknown")]
    [InlineData("3000000000000004", "Unknown")] // Not a recognized prefix
    [InlineData("7000000000000000", "Unknown")]
    [InlineData("8000000000000000", "Unknown")]
    public void Identify_WithUnknownCards_ReturnsUnknown(string cardNumber, string expectedBrand)
    {
        // Act
        var result = _service.Identify(cardNumber);

        // Assert
        result.Should().Be(expectedBrand);
    }

    [Theory]
    [InlineData("", "Unknown")]
    [InlineData(null, "Unknown")]
    [InlineData("   ", "Unknown")]
    public void Identify_WithInvalidInput_ReturnsUnknown(string cardNumber, string expectedBrand)
    {
        // Act
        var result = _service.Identify(cardNumber);

        // Assert
        result.Should().Be(expectedBrand);
    }

    [Theory]
    [InlineData("4532 0151 1283 0366", "Visa")]      // With spaces
    [InlineData("4532-0151-1283-0366", "Visa")]      // With dashes
    [InlineData("4532.0151.1283.0366", "Visa")]      // With dots
    [InlineData("5555/5555/5555/4444", "MasterCard")] // With slashes
    public void Identify_WithFormattedInput_IgnoresNonDigits(string cardNumber, string expectedBrand)
    {
        // Act
        var result = _service.Identify(cardNumber);

        // Assert
        result.Should().Be(expectedBrand);
    }

    [Theory]
    [InlineData("abcdefg", "Unknown")]                 // Only letters
    [InlineData("!@#$%^&", "Unknown")]                // Only special chars
    public void Identify_WithInvalidCharacters_ReturnsUnknown(string cardNumber, string expectedBrand)
    {
        // Act
        var result = _service.Identify(cardNumber);

        // Assert
        result.Should().Be(expectedBrand);
    }

    [Theory]
    [InlineData("Visa", true)]
    [InlineData("MasterCard", true)]
    [InlineData("American Express", true)]
    [InlineData("Discover", true)]
    [InlineData("Elo", true)]
    [InlineData("Hipercard", true)]
    [InlineData("Unknown", false)]
    [InlineData("Invalid", false)]
    [InlineData("SomeRandomBrand", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsSupportedBrand_WithVariousBrands_ReturnsCorrectResult(string brand, bool expected)
    {
        // Act
        var result = _service.IsSupportedBrand(brand);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Identify_WithAllSupportedBrandPrefixes_ReturnsCorrectBrands()
    {
        // Arrange - Test one card for each supported brand
        var testCases = new Dictionary<string, string>
        {
            ["4111111111111111"] = "Visa",
            ["5555555555554444"] = "MasterCard",
            ["378282246310005"] = "American Express", 
            ["6011111111111117"] = "Discover",
            ["5041111111111111"] = "Elo",  // Usar prefixo Elo que não conflita
            ["6062821234567890"] = "Hipercard"
        };

        foreach (var (cardNumber, expectedBrand) in testCases)
        {
            // Act
            var result = _service.Identify(cardNumber);

            // Assert
            result.Should().Be(expectedBrand, $"Card {cardNumber} should be identified as {expectedBrand}");
        }
    }

    [Theory]
    [InlineData("2220", "Unknown")] // Just below MasterCard new range
    [InlineData("2721", "Unknown")] // Just above MasterCard new range
    [InlineData("643", "Unknown")]  // Just below Discover range 644-649
    [InlineData("660", "Unknown")]  // Above Discover range 65x and 644-649
    public void Identify_WithBorderlineValues_ReturnsCorrectResult(string prefix, string expectedBrand)
    {
        // Arrange
        var cardNumber = prefix + "0000000000000"; // Pad to make it long enough

        // Act
        var result = _service.Identify(cardNumber);

        // Assert
        result.Should().Be(expectedBrand);
    }
}
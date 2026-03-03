using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using validacao.Models.Domain;
using validacao.Services.Abstractions;
using validacao.Services.Implementations;
using Xunit;

namespace validacao.Tests.Services.Implementations;

public class CreditCardValidationServiceTests
{
    private readonly Mock<ILuhnValidator> _luhnValidatorMock;
    private readonly Mock<ICardBrandService> _brandServiceMock;
    private readonly Mock<ILogger<CreditCardValidationService>> _loggerMock;
    private readonly CreditCardValidationService _service;

    public CreditCardValidationServiceTests()
    {
        _luhnValidatorMock = new Mock<ILuhnValidator>();
        _brandServiceMock = new Mock<ICardBrandService>();
        _loggerMock = new Mock<ILogger<CreditCardValidationService>>();
        
        _service = new CreditCardValidationService(
            _luhnValidatorMock.Object,
            _brandServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_WithValidCard_ReturnsValidResult()
    {
        // Arrange
        var cardNumber = "4532015112830366";
        _luhnValidatorMock.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
        _brandServiceMock.Setup(x => x.Identify(It.IsAny<string>())).Returns("Visa");

        // Act
        var result = await _service.ValidateAsync(cardNumber);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Brand.Should().Be("Visa");
        result.MaskedCardNumber.Should().Be("************0366");
        result.CardLength.Should().Be(16);
        result.IsSuccess.Should().BeTrue();
        result.ErrorCode.Should().BeNullOrEmpty();
    }

    [Fact]
    public void Validate_WithValidCard_ReturnsValidResult()
    {
        // Arrange
        var cardNumber = "5555555555554444";
        _luhnValidatorMock.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
        _brandServiceMock.Setup(x => x.Identify(It.IsAny<string>())).Returns("MasterCard");

        // Act
        var result = _service.Validate(cardNumber);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Brand.Should().Be("MasterCard");
        result.MaskedCardNumber.Should().Be("************4444");
        result.CardLength.Should().Be(16);
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Validate_WithNullOrEmptyCardNumber_ReturnsError(string cardNumber)
    {
        // Act
        var result = _service.Validate(cardNumber);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Brand.Should().Be("Unknown");
        result.ErrorCode.Should().Be("CARD_NUMBER_REQUIRED");
        result.ErrorMessage.Should().Be("Card number is required");
        result.IsSuccess.Should().BeFalse();
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("!@#$%")]
    [InlineData("abcdef")]
    public void Validate_WithNonNumericCardNumber_ReturnsError(string cardNumber)
    {
        // Act
        var result = _service.Validate(cardNumber);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Brand.Should().Be("Unknown");
        result.ErrorCode.Should().Be("INVALID_CARD_FORMAT");
        result.ErrorMessage.Should().Be("Card number contains no valid digits");
        result.IsSuccess.Should().BeFalse();
    }

    [Theory]
    [InlineData("123")]         // Too short
    [InlineData("123456789012")] // Still too short (12 digits)
    [InlineData("12345678901234567890123")] // Too long (23 digits)
    public void Validate_WithInvalidLength_ReturnsError(string cardNumber)
    {
        // Act
        var result = _service.Validate(cardNumber);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Brand.Should().Be("Unknown");
        result.ErrorCode.Should().Be("INVALID_CARD_LENGTH");
        result.ErrorMessage.Should().Contain("Invalid card length");
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithInvalidLuhnChecksum_ReturnsInvalidResult()
    {
        // Arrange
        var cardNumber = "4532015112830367"; // Invalid Luhn checksum
        _luhnValidatorMock.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);

        // Act
        var result = _service.Validate(cardNumber);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Brand.Should().Be("Invalid");
        result.MaskedCardNumber.Should().Be("************0367");
        result.CardLength.Should().Be(16);
        result.ErrorCode.Should().Be("INVALID_LUHN_CHECKSUM");
        result.ErrorMessage.Should().Be("Invalid card number (Luhn algorithm)");
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithFormattedCardNumber_HandlesCorrectly()
    {
        // Arrange
        var cardNumber = "4532 0151 1283 0366";
        _luhnValidatorMock.Setup(x => x.IsValid("4532015112830366")).Returns(true);
        _brandServiceMock.Setup(x => x.Identify("4532015112830366")).Returns("Visa");

        // Act
        var result = _service.Validate(cardNumber);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Brand.Should().Be("Visa");
        result.MaskedCardNumber.Should().Be("************0366");
        result.CardLength.Should().Be(16);
        
        // Verify mocks were called with cleaned number
        _luhnValidatorMock.Verify(x => x.IsValid("4532015112830366"), Times.Once);
        _brandServiceMock.Verify(x => x.Identify("4532015112830366"), Times.Once);
    }

    [Fact]
    public void Validate_WhenLuhnValidatorThrows_ReturnsErrorResult()
    {
        // Arrange
        var cardNumber = "4532015112830366";
        _luhnValidatorMock.Setup(x => x.IsValid(It.IsAny<string>()))
            .Throws(new InvalidOperationException("Validator error"));

        // Act
        var result = _service.Validate(cardNumber);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Brand.Should().Be("Unknown");
        result.ErrorCode.Should().Be("VALIDATION_ERROR");
        result.ErrorMessage.Should().Be("Internal validation error");
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Validate_WhenBrandServiceThrows_ReturnsErrorResult()
    {
        // Arrange
        var cardNumber = "4532015112830366";
        _luhnValidatorMock.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
        _brandServiceMock.Setup(x => x.Identify(It.IsAny<string>()))
            .Throws(new InvalidOperationException("Brand service error"));

        // Act
        var result = _service.Validate(cardNumber);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Brand.Should().Be("Unknown");
        result.ErrorCode.Should().Be("VALIDATION_ERROR");
        result.ErrorMessage.Should().Be("Internal validation error");
        result.IsSuccess.Should().BeFalse();
    }

    [Theory]
    [InlineData("4532015112830366", "Visa")]
    [InlineData("5555555555554444", "MasterCard")]
    [InlineData("378282246310005", "American Express")]
    [InlineData("6011111111111117", "Discover")]
    public void Validate_WithDifferentBrands_ReturnsCorrectBrand(string cardNumber, string expectedBrand)
    {
        // Arrange
        _luhnValidatorMock.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
        _brandServiceMock.Setup(x => x.Identify(It.IsAny<string>())).Returns(expectedBrand);

        // Act
        var result = _service.Validate(cardNumber);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Brand.Should().Be(expectedBrand);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Validate_LogsValidationStart()
    {
        // Arrange
        var cardNumber = "4532015112830366";
        _luhnValidatorMock.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
        _brandServiceMock.Setup(x => x.Identify(It.IsAny<string>())).Returns("Visa");

        // Act
        _service.Validate(cardNumber);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Iniciando validação de cartão")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public void Validate_LogsValidationSuccess()
    {
        // Arrange
        var cardNumber = "4532015112830366";
        _luhnValidatorMock.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
        _brandServiceMock.Setup(x => x.Identify(It.IsAny<string>())).Returns("Visa");

        // Act
        _service.Validate(cardNumber);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Cartão validado com sucesso")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public void Validate_LogsValidationFailure()
    {
        // Arrange
        var cardNumber = "4532015112830367"; // Invalid
        _luhnValidatorMock.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);

        // Act
        _service.Validate(cardNumber);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Cartão falhou na validação de Luhn")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public void Validate_LogsException()
    {
        // Arrange
        var cardNumber = "4532015112830366";
        var exception = new InvalidOperationException("Test exception");
        _luhnValidatorMock.Setup(x => x.IsValid(It.IsAny<string>())).Throws(exception);

        // Act
        _service.Validate(cardNumber);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Erro inesperado durante validação")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public void Constructor_WithNullDependencies_ThrowsArgumentNullException()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() => 
            new CreditCardValidationService(null, _brandServiceMock.Object, _loggerMock.Object));
        
        Assert.Throws<ArgumentNullException>(() => 
            new CreditCardValidationService(_luhnValidatorMock.Object, null, _loggerMock.Object));
        
        Assert.Throws<ArgumentNullException>(() => 
            new CreditCardValidationService(_luhnValidatorMock.Object, _brandServiceMock.Object, null));
    }

    [Fact]
    public void Validate_WithMixedCharactersButValidDigits_ProcessesCorrectly()
    {
        // Arrange
        var cardNumber = "4532-abc-0151-def-1283-ghi-0366";
        _luhnValidatorMock.Setup(x => x.IsValid("4532015112830366")).Returns(true);
        _brandServiceMock.Setup(x => x.Identify("4532015112830366")).Returns("Visa");

        // Act
        var result = _service.Validate(cardNumber);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Brand.Should().Be("Visa");
        result.MaskedCardNumber.Should().Be("************0366");
        result.CardLength.Should().Be(16);
    }

    [Fact]
    public async Task ValidateAsync_DelegatesToSynchronousValidate()
    {
        // Arrange
        var cardNumber = "4532015112830366";
        _luhnValidatorMock.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
        _brandServiceMock.Setup(x => x.Identify(It.IsAny<string>())).Returns("Visa");

        // Act
        var asyncResult = await _service.ValidateAsync(cardNumber);
        var syncResult = _service.Validate(cardNumber);

        // Assert
        asyncResult.Should().BeEquivalentTo(syncResult);
    }
}
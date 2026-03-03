using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using validacao.Controllers;
using validacao.Models.Domain;
using validacao.Models.Requests;
using validacao.Models.Responses;
using validacao.Services.Abstractions;
using Xunit;

namespace validacao.Tests.Controllers;

public class CreditCardControllerTests
{
    private readonly Mock<ICreditCardValidationService> _validationServiceMock;
    private readonly Mock<ILogger<CreditCardController>> _loggerMock;
    private readonly CreditCardController _controller;

    public CreditCardControllerTests()
    {
        _validationServiceMock = new Mock<ICreditCardValidationService>();
        _loggerMock = new Mock<ILogger<CreditCardController>>();
        _controller = new CreditCardController(_validationServiceMock.Object, _loggerMock.Object);
        
        // Setup HttpContext for TraceIdentifier
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.HttpContext.TraceIdentifier = "test-trace-id";
    }

    [Fact]
    public async Task ValidateAsync_WithValidCard_ReturnsOkResult()
    {
        // Arrange
        var request = new CreditCardRequest { CardNumber = "4532015112830366" };
        var validationResult = new CreditCardValidationResult
        {
            IsValid = true,
            Brand = "Visa",
            MaskedCardNumber = "************0366",
            CardLength = 16
        };

        _validationServiceMock.Setup(x => x.ValidateAsync(request.CardNumber))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _controller.ValidateAsync(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var response = okResult!.Value as CreditCardResponse;
        
        response.Should().NotBeNull();
        response!.CardNumber.Should().Be("************0366");
        response.IsValid.Should().BeTrue();
        response.Brand.Should().Be("Visa");
        response.Message.Should().Be("Cartão válido");
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidCard_ReturnsBadRequestResult()
    {
        // Arrange
        var request = new CreditCardRequest { CardNumber = "1234567890123456" };
        var validationResult = new CreditCardValidationResult
        {
            IsValid = false,
            Brand = "Invalid",
            MaskedCardNumber = "************3456",
            ErrorMessage = "Invalid card number (Luhn algorithm)",
            ErrorCode = "INVALID_LUHN_CHECKSUM"
        };

        _validationServiceMock.Setup(x => x.ValidateAsync(request.CardNumber))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _controller.ValidateAsync(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        var response = badRequestResult!.Value as CreditCardResponse;
        
        response.Should().NotBeNull();
        response!.CardNumber.Should().Be("************3456");
        response.IsValid.Should().BeFalse();
        response.Brand.Should().Be("Invalid");
        response.Message.Should().Be("Invalid card number (Luhn algorithm)");
    }

    [Fact]
    public async Task ValidateAsync_WithNullRequest_ReturnsBadRequestWithError()
    {
        // Act
        var result = await _controller.ValidateAsync(null);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        var response = badRequestResult!.Value as ErrorResponse;
        
        response.Should().NotBeNull();
        response!.ErrorCode.Should().Be("INVALID_REQUEST");
        response.Message.Should().Be("Request body is required");
        response.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ValidateAsync_WithEmptyCardNumber_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreditCardRequest { CardNumber = "" };
        var validationResult = new CreditCardValidationResult
        {
            IsValid = false,
            Brand = "Unknown",
            ErrorMessage = "Card number is required",
            ErrorCode = "CARD_NUMBER_REQUIRED"
        };

        _validationServiceMock.Setup(x => x.ValidateAsync(request.CardNumber))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _controller.ValidateAsync(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        var response = badRequestResult!.Value as CreditCardResponse;
        
        response.Should().NotBeNull();
        response!.IsValid.Should().BeFalse();
        response.Message.Should().Be("Card number is required");
    }

    [Fact]
    public async Task ValidateAsync_WhenValidationServiceThrows_ReturnsInternalServerError()
    {
        // Arrange
        var request = new CreditCardRequest { CardNumber = "4532015112830366" };
        var exception = new InvalidOperationException("Service error");
        
        _validationServiceMock.Setup(x => x.ValidateAsync(request.CardNumber))
            .ThrowsAsync(exception);

        // Act
        var result = await _controller.ValidateAsync(request);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        
        var response = objectResult.Value as ErrorResponse;
        response.Should().NotBeNull();
        response!.ErrorCode.Should().Be("INTERNAL_SERVER_ERROR");
        response.Message.Should().Be("An internal server error occurred during validation");
        response.TraceId.Should().Be("test-trace-id");
    }

    [Fact]
    public async Task ValidateAsync_LogsRequestReceived()
    {
        // Arrange
        var request = new CreditCardRequest { CardNumber = "4532015112830366" };
        var validationResult = new CreditCardValidationResult
        {
            IsValid = true,
            Brand = "Visa",
            MaskedCardNumber = "************0366"
        };

        _validationServiceMock.Setup(x => x.ValidateAsync(request.CardNumber))
            .ReturnsAsync(validationResult);

        // Act
        await _controller.ValidateAsync(request);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Requisição de validação recebida")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ValidateAsync_LogsSuccessfulValidation()
    {
        // Arrange
        var request = new CreditCardRequest { CardNumber = "4532015112830366" };
        var validationResult = new CreditCardValidationResult
        {
            IsValid = true,
            Brand = "Visa",
            MaskedCardNumber = "************0366"
        };

        _validationServiceMock.Setup(x => x.ValidateAsync(request.CardNumber))
            .ReturnsAsync(validationResult);

        // Act
        await _controller.ValidateAsync(request);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Validação realizada com sucesso")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ValidateAsync_LogsValidationFailure()
    {
        // Arrange
        var request = new CreditCardRequest { CardNumber = "1234567890123456" };
        var validationResult = new CreditCardValidationResult
        {
            IsValid = false,
            ErrorCode = "INVALID_LUHN_CHECKSUM"
        };

        _validationServiceMock.Setup(x => x.ValidateAsync(request.CardNumber))
            .ReturnsAsync(validationResult);

        // Act
        await _controller.ValidateAsync(request);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Validação falhou")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ValidateAsync_LogsException()
    {
        // Arrange
        var request = new CreditCardRequest { CardNumber = "4532015112830366" };
        var exception = new InvalidOperationException("Test exception");
        
        _validationServiceMock.Setup(x => x.ValidateAsync(request.CardNumber))
            .ThrowsAsync(exception);

        // Act
        await _controller.ValidateAsync(request);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Erro inesperado na validação")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ValidateAsync_WithDifferentBrands_ReturnsCorrectBrand()
    {
        // Arrange
        var testCases = new[]
        {
            new { CardNumber = "4532015112830366", Brand = "Visa" },
            new { CardNumber = "5555555555554444", Brand = "MasterCard" },
            new { CardNumber = "378282246310005", Brand = "American Express" }
        };

        foreach (var testCase in testCases)
        {
            var request = new CreditCardRequest { CardNumber = testCase.CardNumber };
            var validationResult = new CreditCardValidationResult
            {
                IsValid = true,
                Brand = testCase.Brand,
                MaskedCardNumber = "************" + testCase.CardNumber.Substring(testCase.CardNumber.Length - 4)
            };

            _validationServiceMock.Setup(x => x.ValidateAsync(testCase.CardNumber))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.ValidateAsync(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as CreditCardResponse;
            response!.Brand.Should().Be(testCase.Brand);
        }
    }

    [Fact]
    public void Constructor_WithNullDependencies_ThrowsArgumentNullException()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() => 
            new CreditCardController(null, _loggerMock.Object));
        
        Assert.Throws<ArgumentNullException>(() => 
            new CreditCardController(_validationServiceMock.Object, null));
    }

    [Fact]
    public async Task ValidateAsync_WithFormattedCardNumber_ProcessesCorrectly()
    {
        // Arrange
        var request = new CreditCardRequest { CardNumber = "4532 0151 1283 0366" };
        var validationResult = new CreditCardValidationResult
        {
            IsValid = true,
            Brand = "Visa",
            MaskedCardNumber = "************0366",
            CardLength = 16
        };

        _validationServiceMock.Setup(x => x.ValidateAsync(request.CardNumber))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _controller.ValidateAsync(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var response = okResult!.Value as CreditCardResponse;
        
        response.Should().NotBeNull();
        response!.IsValid.Should().BeTrue();
        response.Brand.Should().Be("Visa");
        
        // Verify service was called with the original input (service handles formatting)
        _validationServiceMock.Verify(x => x.ValidateAsync("4532 0151 1283 0366"), Times.Once);
    }

    [Fact]
    public async Task ValidateAsync_ReturnsCorrectStatusCodes()
    {
        // Test OK (200)
        var validRequest = new CreditCardRequest { CardNumber = "4532015112830366" };
        _validationServiceMock.Setup(x => x.ValidateAsync(It.IsAny<string>()))
            .ReturnsAsync(new CreditCardValidationResult { IsValid = true, Brand = "Visa" });
        
        var validResult = await _controller.ValidateAsync(validRequest);
        validResult.Should().BeOfType<OkObjectResult>();
        
        // Test BadRequest (400) for invalid card
        var invalidRequest = new CreditCardRequest { CardNumber = "1234567890123456" };
        _validationServiceMock.Setup(x => x.ValidateAsync(It.IsAny<string>()))
            .ReturnsAsync(new CreditCardValidationResult { IsValid = false });
        
        var invalidResult = await _controller.ValidateAsync(invalidRequest);
        invalidResult.Should().BeOfType<BadRequestObjectResult>();
        
        // Test BadRequest (400) for null request
        var nullResult = await _controller.ValidateAsync(null);
        nullResult.Should().BeOfType<BadRequestObjectResult>();
    }
}
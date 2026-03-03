using validacao.Models.Domain;
using validacao.Services.Abstractions;
using validacao.Services.Utilities;

namespace validacao.Services.Implementations;

/// <summary>
/// Serviço principal para validação completa de cartões de crédito
/// </summary>
public class CreditCardValidationService : ICreditCardValidationService
{
    private readonly ILuhnValidator _luhnValidator;
    private readonly ICardBrandService _brandService;
    private readonly ILogger<CreditCardValidationService> _logger;

    public CreditCardValidationService(
        ILuhnValidator luhnValidator,
        ICardBrandService brandService,
        ILogger<CreditCardValidationService> logger)
    {
        _luhnValidator = luhnValidator ?? throw new ArgumentNullException(nameof(luhnValidator));
        _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CreditCardValidationResult> ValidateAsync(string cardNumber)
    {
        // Para operações mais complexas no futuro (ex: validação contra blacklist externa)
        return await Task.FromResult(Validate(cardNumber));
    }

    public CreditCardValidationResult Validate(string cardNumber)
    {
        _logger.LogInformation("Iniciando validação de cartão");

        // Validação básica de entrada
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            _logger.LogWarning("Tentativa de validação com cartão vazio ou nulo");
            return CreateErrorResult("Card number is required", "CARD_NUMBER_REQUIRED");
        }

        // Limpar e validar formato
        var cleanedNumber = CardNumberFormatter.Clean(cardNumber);
        
        if (string.IsNullOrEmpty(cleanedNumber))
        {
            _logger.LogWarning("Cartão contém apenas caracteres inválidos");
            return CreateErrorResult("Card number contains no valid digits", "INVALID_CARD_FORMAT");
        }

        if (!CardNumberFormatter.IsValidLength(cleanedNumber))
        {
            _logger.LogWarning("Comprimento do cartão inválido: {Length}", cleanedNumber.Length);
            return CreateErrorResult($"Invalid card length: {cleanedNumber.Length}. Must be between 13-19 digits", "INVALID_CARD_LENGTH");
        }

        // Validar se contém apenas números
        if (!long.TryParse(cleanedNumber, out _))
        {
            _logger.LogWarning("Cartão contém caracteres não numéricos após limpeza");
            return CreateErrorResult("Card number must contain only digits", "INVALID_CARD_FORMAT");
        }

        try
        {
            // Validação pelo algoritmo de Luhn
            var isValidLuhn = _luhnValidator.IsValid(cleanedNumber);
            
            if (!isValidLuhn)
            {
                _logger.LogInformation("Cartão falhou na validação de Luhn");
                return new CreditCardValidationResult
                {
                    IsValid = false,
                    Brand = "Invalid",
                    MaskedCardNumber = CardNumberFormatter.Mask(cleanedNumber),
                    CardLength = cleanedNumber.Length,
                    ErrorMessage = "Invalid card number (Luhn algorithm)",
                    ErrorCode = "INVALID_LUHN_CHECKSUM"
                };
            }

            // Identificar bandeira
            var brand = _brandService.Identify(cleanedNumber);
            
            _logger.LogInformation("Cartão validado com sucesso - Brand: {Brand}, Length: {Length}", brand, cleanedNumber.Length);

            return new CreditCardValidationResult
            {
                IsValid = true,
                Brand = brand,
                MaskedCardNumber = CardNumberFormatter.Mask(cleanedNumber),
                CardLength = cleanedNumber.Length
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado durante validação do cartão");
            return CreateErrorResult("Internal validation error", "VALIDATION_ERROR");
        }
    }

    private static CreditCardValidationResult CreateErrorResult(string message, string errorCode)
    {
        return new CreditCardValidationResult
        {
            IsValid = false,
            Brand = "Unknown",
            MaskedCardNumber = string.Empty,
            ErrorMessage = message,
            ErrorCode = errorCode
        };
    }
}
using validacao.Models.Domain;

namespace validacao.Services.Abstractions;

/// <summary>
/// Interface principal para validação de cartões de crédito
/// </summary>
public interface ICreditCardValidationService
{
    /// <summary>
    /// Valida completamente um cartão de crédito
    /// </summary>
    /// <param name="cardNumber">Número do cartão (pode conter espaços e caracteres especiais)</param>
    /// <returns>Resultado completo da validação</returns>
    Task<CreditCardValidationResult> ValidateAsync(string cardNumber);

    /// <summary>
    /// Valida completamente um cartão de crédito (versão síncrona)
    /// </summary>
    /// <param name="cardNumber">Número do cartão (pode conter espaços e caracteres especiais)</param>
    /// <returns>Resultado completo da validação</returns>
    CreditCardValidationResult Validate(string cardNumber);
}
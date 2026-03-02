namespace validacao.Models.Domain;

/// <summary>
/// Representa o resultado da validação de um cartão de crédito
/// </summary>
public class CreditCardValidationResult
{
    /// <summary>
    /// Indica se o cartão é válido pelo algoritmo de Luhn
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Bandeira identificada do cartão
    /// </summary>
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    /// Número do cartão mascarado (apenas últimos 4 dígitos visíveis)
    /// </summary>
    public string MaskedCardNumber { get; set; } = string.Empty;

    /// <summary>
    /// Mensagem de erro se a validação falhar
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Código de erro para categorização
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Comprimento do cartão após limpeza
    /// </summary>
    public int CardLength { get; set; }

    /// <summary>
    /// Indica se houve sucesso na validação (sem erros)
    /// </summary>
    public bool IsSuccess => string.IsNullOrEmpty(ErrorCode);
}
namespace validacao.Models.Responses;

/// <summary>
/// Modelo de resposta para erros da API
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Código do erro para categorização
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// Mensagem descritiva do erro
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Detalhes adicionais sobre o erro (opcional)
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Erros de validação por campo (para formulários)
    /// </summary>
    public Dictionary<string, string[]>? ValidationErrors { get; set; }

    /// <summary>
    /// Timestamp do erro
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ID único para rastreamento do erro (para logs)
    /// </summary>
    public string? TraceId { get; set; }
}
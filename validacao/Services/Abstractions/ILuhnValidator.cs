namespace validacao.Services.Abstractions;

/// <summary>
/// Interface para validação do algoritmo de Luhn
/// </summary>
public interface ILuhnValidator
{
    /// <summary>
    /// Valida se o número do cartão é válido pelo algoritmo de Luhn
    /// </summary>
    /// <param name="cardNumber">Número do cartão limpo (apenas dígitos)</param>
    /// <returns>True se válido, False caso contrário</returns>
    bool IsValid(string cardNumber);
}
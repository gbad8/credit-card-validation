namespace validacao.Services.Abstractions;

/// <summary>
/// Interface para identificação de bandeiras de cartão
/// </summary>
public interface ICardBrandService
{
    /// <summary>
    /// Identifica a bandeira do cartão baseado no número
    /// </summary>
    /// <param name="cardNumber">Número do cartão limpo (apenas dígitos)</param>
    /// <returns>Nome da bandeira identificada ou "Unknown"</returns>
    string Identify(string cardNumber);

    /// <summary>
    /// Verifica se a bandeira é suportada pelo sistema
    /// </summary>
    /// <param name="brand">Nome da bandeira</param>
    /// <returns>True se suportada</returns>
    bool IsSupportedBrand(string brand);
}
using System.Text.RegularExpressions;

namespace validacao.Services.Utilities;

/// <summary>
/// Utilitário para formatação e limpeza de números de cartão
/// </summary>
public static class CardNumberFormatter
{
    /// <summary>
    /// Remove todos os caracteres não numéricos do número do cartão
    /// </summary>
    /// <param name="cardNumber">Número do cartão com possíveis espaços e caracteres especiais</param>
    /// <returns>Apenas os dígitos do cartão</returns>
    public static string Clean(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return string.Empty;
        
        return Regex.Replace(cardNumber, @"[^\d]", "");
    }

    /// <summary>
    /// Verifica se o comprimento do cartão está dentro do padrão válido
    /// </summary>
    /// <param name="cardNumber">Número do cartão limpo (apenas dígitos)</param>
    /// <returns>True se o comprimento estiver entre 13 e 19 dígitos</returns>
    public static bool IsValidLength(string cardNumber)
    {
        return !string.IsNullOrEmpty(cardNumber) && 
               cardNumber.Length >= 13 && 
               cardNumber.Length <= 19;
    }

    /// <summary>
    /// Mascara o número do cartão mostrando apenas os últimos dígitos
    /// </summary>
    /// <param name="cardNumber">Número do cartão</param>
    /// <param name="visibleDigits">Quantidade de dígitos visíveis no final</param>
    /// <returns>Cartão mascarado com asteriscos</returns>
    public static string Mask(string cardNumber, int visibleDigits = 4)
    {
        if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length <= visibleDigits)
            return cardNumber;
        
        var visible = cardNumber.Substring(cardNumber.Length - visibleDigits);
        var masked = new string('*', cardNumber.Length - visibleDigits);
        
        return $"{masked}{visible}";
    }

    /// <summary>
    /// Formata o número do cartão com espaços para melhor legibilidade
    /// </summary>
    /// <param name="cardNumber">Número do cartão limpo</param>
    /// <returns>Cartão formatado com espaços a cada 4 dígitos</returns>
    public static string Format(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return string.Empty;

        return Regex.Replace(cardNumber, @"(.{4})", "$1 ").Trim();
    }
}
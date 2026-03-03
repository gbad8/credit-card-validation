using validacao.Services.Abstractions;
using validacao.Services.Utilities;

namespace validacao.Services.Implementations;

public class CardBrandService : ICardBrandService
{
    private static readonly HashSet<string> SupportedBrands = new()
    {
        "Visa", "MasterCard", "American Express", "Discover", "Elo", "Hipercard"
    };

    private static readonly string[] EloPrefixes =
    {
        "4011", "4312", "4389", "4514", "4576",
        "5041", "5067", "5090", "6277", "6362", "6363"
    };

    public string Identify(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return "Unknown";

        // Limpar cartão usando utilitário
        var cleanedNumber = CardNumberFormatter.Clean(cardNumber);

        if (string.IsNullOrEmpty(cleanedNumber) || !long.TryParse(cleanedNumber, out _))
            return "Unknown";

        // VISA
        if (cleanedNumber.StartsWith("4"))
            return "Visa";

        // AMERICAN EXPRESS
        if (cleanedNumber.StartsWith("34") || cleanedNumber.StartsWith("37"))
            return "American Express";

        // HIPERCARD
        if (cleanedNumber.StartsWith("606282"))
            return "Hipercard";

        // DISCOVER
        if (cleanedNumber.StartsWith("6011") ||
            cleanedNumber.StartsWith("65") ||
            (cleanedNumber.Length >= 3 &&
             int.TryParse(cleanedNumber.Substring(0, 3), out int d3) &&
             d3 >= 644 && d3 <= 649))
        {
            return "Discover";
        }

        // MASTERCARD (51–55)
        if (cleanedNumber.Length >= 2 &&
            int.TryParse(cleanedNumber.Substring(0, 2), out int mc2) &&
            mc2 >= 51 && mc2 <= 55)
        {
            return "MasterCard";
        }

        // MASTERCARD (2221–2720)
        if (cleanedNumber.Length >= 4 &&
            int.TryParse(cleanedNumber.Substring(0, 4), out int mc4) &&
            mc4 >= 2221 && mc4 <= 2720)
        {
            return "MasterCard";
        }

        // ELO (principais prefixos)
        foreach (var prefix in EloPrefixes)
        {
            if (cleanedNumber.StartsWith(prefix))
                return "Elo";
        }

        return "Unknown";
    }

    public bool IsSupportedBrand(string brand)
    {
        return SupportedBrands.Contains(brand);
    }
}

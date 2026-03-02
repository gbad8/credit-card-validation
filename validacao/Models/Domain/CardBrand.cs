namespace validacao.Models.Domain;

/// <summary>
/// Enum que representa as bandeiras de cartão suportadas
/// </summary>
public enum CardBrand
{
    Unknown,
    Visa,
    MasterCard,
    AmericanExpress,
    Discover,
    Elo,
    Hipercard,
    Invalid
}

/// <summary>
/// Extensões para o enum CardBrand
/// </summary>
public static class CardBrandExtensions
{
    /// <summary>
    /// Converte o enum para string amigável
    /// </summary>
    public static string ToFriendlyString(this CardBrand brand)
    {
        return brand switch
        {
            CardBrand.Visa => "Visa",
            CardBrand.MasterCard => "MasterCard",
            CardBrand.AmericanExpress => "American Express",
            CardBrand.Discover => "Discover",
            CardBrand.Elo => "Elo",
            CardBrand.Hipercard => "Hipercard",
            CardBrand.Invalid => "Invalid",
            _ => "Unknown"
        };
    }

    /// <summary>
    /// Converte string para enum CardBrand
    /// </summary>
    public static CardBrand FromString(string brandName)
    {
        return brandName?.ToLowerInvariant() switch
        {
            "visa" => CardBrand.Visa,
            "mastercard" => CardBrand.MasterCard,
            "american express" => CardBrand.AmericanExpress,
            "discover" => CardBrand.Discover,
            "elo" => CardBrand.Elo,
            "hipercard" => CardBrand.Hipercard,
            "invalid" => CardBrand.Invalid,
            _ => CardBrand.Unknown
        };
    }
}
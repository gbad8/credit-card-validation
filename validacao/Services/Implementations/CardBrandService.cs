namespace validacao.Services.Implementations
{
    public static class CardBrandService
    {
        public static string Identify(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return "Unknown";

            // Remove espaços
            cardNumber = new string(cardNumber.Where(char.IsDigit).ToArray());

            // Garante que só existem números
            if (!long.TryParse(cardNumber, out _))
                return "Unknown";

            // VISA
            if (cardNumber.StartsWith("4"))
                return "Visa";

            // AMERICAN EXPRESS
            if (cardNumber.StartsWith("34") || cardNumber.StartsWith("37"))
                return "American Express";

            // HIPERCARD
            if (cardNumber.StartsWith("606282"))
                return "Hipercard";

            // DISCOVER
            if (cardNumber.StartsWith("6011") ||
                cardNumber.StartsWith("65") ||
                (cardNumber.Length >= 3 &&
                 int.TryParse(cardNumber.Substring(0, 3), out int d3) &&
                 d3 >= 644 && d3 <= 649))
            {
                return "Discover";
            }

            // MASTERCARD (51–55)
            if (cardNumber.Length >= 2 &&
                int.TryParse(cardNumber.Substring(0, 2), out int mc2) &&
                mc2 >= 51 && mc2 <= 55)
            {
                return "MasterCard";
            }

            // MASTERCARD (2221–2720)
            if (cardNumber.Length >= 4 &&
                int.TryParse(cardNumber.Substring(0, 4), out int mc4) &&
                mc4 >= 2221 && mc4 <= 2720)
            {
                return "MasterCard";
            }

            // ELO (principais prefixos)
            string[] eloPrefixes =
            {
                "4011", "4312", "4389", "4514", "4576",
                "5041", "5067", "5090", "6277", "6362", "6363"
            };

            foreach (var prefix in eloPrefixes)
            {
                if (cardNumber.StartsWith(prefix))
                    return "Elo";
            }

            return "Unknown";
        }
    }
}

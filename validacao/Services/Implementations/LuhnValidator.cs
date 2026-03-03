using validacao.Services.Abstractions;
using validacao.Services.Utilities;

namespace validacao.Services.Implementations;

public class LuhnValidator : ILuhnValidator
{
    public bool IsValid(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return false;

        // Limpar e validar o cartão usando utilitário
        var cleanedNumber = CardNumberFormatter.Clean(cardNumber);
        
        if (string.IsNullOrEmpty(cleanedNumber) || !CardNumberFormatter.IsValidLength(cleanedNumber))
            return false;

        int sum = 0;
        bool alternate = false;

        for (int i = cleanedNumber.Length - 1; i >= 0; i--)
        {
            int n = cleanedNumber[i] - '0'; 

            if (alternate)
            {
                n *= 2;
                if (n > 9)
                    n -= 9;
            }

            sum += n;
            alternate = !alternate;
        }

        return sum % 10 == 0;
    }
}

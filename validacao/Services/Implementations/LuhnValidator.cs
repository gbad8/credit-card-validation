using System.Linq;

namespace validacao.Services.Implementations;

public static class LuhnValidator
{
    public static bool IsValid(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return false;

        // 🔹 REMOVE TUDO QUE NÃO FOR NÚMERO
        cardNumber = new string(cardNumber.Where(char.IsDigit).ToArray());

        if (string.IsNullOrEmpty(cardNumber))
            return false;

        int sum = 0;
        bool alternate = false;

        for (int i = cardNumber.Length - 1; i >= 0; i--)
        {
            int n = cardNumber[i] - '0'; 

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

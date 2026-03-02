namespace validacao.Models.Responses
{
    public class CreditCardResponse
    {
        public string CardNumber { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}

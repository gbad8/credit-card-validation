using Microsoft.AspNetCore.Mvc;
using validacao.Models.Requests;
using validacao.Models.Responses;
using validacao.Services.Implementations;

namespace CreditCard.Api.Controllers
{
    [ApiController]
    [Route("api/creditcard")]
    public class CreditCardController : ControllerBase
    {
        [HttpPost("validate")]
        public IActionResult Validate([FromBody] CreditCardRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.CardNumber))
            {
                return BadRequest(new CreditCardResponse
                {
                    Message = "Número do cartão é obrigatório"
                });
            }

            bool isValid = LuhnValidator.IsValid(request.CardNumber);
            string brand = isValid
                ? CardBrandService.Identify(request.CardNumber)
                : "Invalid";

            var response = new CreditCardResponse
            {
                CardNumber = request.CardNumber,
                IsValid = isValid,
                Brand = brand,
                Message = isValid ? "Cartão válido" : "Cartão inválido"
            };

            return Ok(response);
        }
    }
}

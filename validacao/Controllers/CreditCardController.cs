using Microsoft.AspNetCore.Mvc;
using validacao.Models.Requests;
using validacao.Models.Responses;
using validacao.Services.Abstractions;
using validacao.Services.Utilities;

namespace validacao.Controllers;

/// <summary>
/// Controller para validação de cartões de crédito
/// </summary>
[ApiController]
[Route("api/creditcard")]
[Produces("application/json")]
public class CreditCardController : ControllerBase
{
    private readonly ICreditCardValidationService _validationService;
    private readonly ILogger<CreditCardController> _logger;

    public CreditCardController(
        ICreditCardValidationService validationService,
        ILogger<CreditCardController> logger)
    {
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Valida um número de cartão de crédito
    /// </summary>
    /// <param name="request">Dados da requisição contendo o número do cartão</param>
    /// <returns>Resultado da validação</returns>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(CreditCardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ValidateAsync([FromBody] CreditCardRequest request)
    {
        _logger.LogInformation("Requisição de validação recebida");

        // Validação básica da requisição
        if (request == null)
        {
            _logger.LogWarning("Requisição nula recebida");
            return BadRequest(new ErrorResponse
            {
                ErrorCode = "INVALID_REQUEST",
                Message = "Request body is required",
                Timestamp = DateTime.UtcNow
            });
        }

        try
        {
            // Delegar toda a lógica para o serviço
            var validationResult = await _validationService.ValidateAsync(request.CardNumber);

            // Converter resultado do domínio para resposta da API
            var response = new CreditCardResponse
            {
                CardNumber = validationResult.MaskedCardNumber,
                IsValid = validationResult.IsValid,
                Brand = validationResult.Brand,
                Message = validationResult.IsValid 
                    ? "Cartão válido" 
                    : validationResult.ErrorMessage ?? "Cartão inválido"
            };

            // Se inválido, retornar BadRequest
            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validação falhou: {ErrorCode}", validationResult.ErrorCode);
                return BadRequest(response);
            }

            _logger.LogInformation("Validação realizada com sucesso");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado na validação");
            
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
            {
                ErrorCode = "INTERNAL_SERVER_ERROR",
                Message = "An internal server error occurred during validation",
                Timestamp = DateTime.UtcNow,
                TraceId = HttpContext.TraceIdentifier
            });
        }
    }
}

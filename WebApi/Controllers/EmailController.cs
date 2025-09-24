using Application.Service;
using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
  [Route("email")]
  [ApiController]
  [Authorize]
  public class EmailController : Controller
  {
    private readonly IEmailService _service;
    private readonly IQueueManagerService _messageService;
    private readonly ILogger _logger;

    public EmailController(IEmailService service, IQueueManagerService messageService, ILogger logger) {
      _service = service;
      _messageService = messageService;
      _logger = logger;
    }
    /// <summary>
    /// Endpoint responsável por enviar um email para um endereço de email especifico.
    /// </summary>
    /// <returns>Valida os dados passados para envio do email e envia o mesmo para o endereço de email de destino.</returns>
    [HttpPost("send")]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status417ExpectationFailed)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Response<bool>>> CreateSession([FromBody] SendEmailRequest sendEmailRequest)
    {
      try
      {
        var response = await _messageService.Publish(sendEmailRequest, "send-mail-qu");
        return StatusCode(StatusCodes.Status200OK, new Response<bool>() { Status = 200, Message = $"Email enviado com sucesso.", Data = response, Success = true });
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "Exception while send email!");
        switch (ex.Message)
        {
          case "errorInEmailSent":
            return StatusCode(StatusCodes.Status417ExpectationFailed, new Response<bool>() { Status = 403, Message = $"Não foi possível enviar email. {ex.Message}", Success = false });
          default:
            return StatusCode(StatusCodes.Status500InternalServerError, new Response<bool>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false });
        }   
      }
    }
  }
}

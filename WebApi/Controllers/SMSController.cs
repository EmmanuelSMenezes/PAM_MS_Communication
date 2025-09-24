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
  [Route("sms")]
  [ApiController]
  public class SMSController : Controller
  {
    private readonly ISMSService _service;
    private readonly IQueueManagerService _messageService;
    private readonly ILogger _logger;

    public SMSController(ISMSService service, IQueueManagerService messageService, ILogger logger) {
      _service = service;
      _logger = logger;
      _messageService = messageService;
    }

    /// <summary>
    /// Endpoint responsável por enviar um sms para um numero especifico de telefone.
    /// </summary>
    /// <returns>Valida os dados passados para envio do sms e envia o mesmo para o numero de destino.</returns>
    [Authorize]
    [HttpPost("send")]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status417ExpectationFailed)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Response<bool>>> CreateSession([FromBody] SendSMSRequest sendSMSRequest)
    {
      try
      {
        var response = await _messageService.Publish(sendSMSRequest, "send-sms-qu");
        return StatusCode(StatusCodes.Status200OK, new Response<bool>() { Status = 200, Message = $"SMS enviado com sucesso.", Data = response, Success = true });
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "Exception while send sms!");
        switch (ex.Message)
        {
          case "errorInSMSSent":
            return StatusCode(StatusCodes.Status417ExpectationFailed, new Response<bool>() { Status = 403, Message = $"Não foi possível enviar sms. {ex.Message}", Success = false });
          default:
            return StatusCode(StatusCodes.Status500InternalServerError, new Response<bool>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false });
        }   
      }
    }
  }
}

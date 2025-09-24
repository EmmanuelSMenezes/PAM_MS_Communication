using Application.Service;
using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
  [Route("message")]
  [ApiController]
  public class MessageController : Controller
  {
    private readonly IMessageService _service;
    private readonly ILogger _logger;

    public MessageController(IMessageService service, ILogger logger)
    {
      _service = service;
      _logger = logger;
    }

    /// <summary>
    /// Endpoint responsável por atualizar uma mensagem.
    /// </summary>
    /// <returns>Valida os dados passados para atualização da mensagem.</returns>
    [HttpPut("update")]
    [ProducesResponseType(typeof(Response<Message>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<Message>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Response<Message>), StatusCodes.Status417ExpectationFailed)]
    [ProducesResponseType(typeof(Response<Message>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Response<Message>), StatusCodes.Status500InternalServerError)]
    public ActionResult<Response<Message>> CreateSession([FromBody] UpdateMessageRequest updateMessageRequest)
    {
      try
      {
        var response = _service.UpdateMessage(updateMessageRequest);
        return StatusCode(StatusCodes.Status200OK, new Response<Message>() { Status = 200, Message = $"Chat criado com sucesso.", Data = response, Success = true });
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "Exception while send sms!");
        switch (ex.Message)
        {
          case "errorInCreateChat":
            return StatusCode(StatusCodes.Status417ExpectationFailed, new Response<CreateChatResponse>() { Status = 403, Message = $"Não foi possível criar chat. {ex.Message}", Success = false });
          default:
            return StatusCode(StatusCodes.Status500InternalServerError, new Response<CreateChatResponse>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false });
        }
      }
    }
  }
}

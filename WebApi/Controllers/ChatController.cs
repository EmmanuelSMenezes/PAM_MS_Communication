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
  [Route("chat")]
  [ApiController]
  [AllowAnonymous]
  public class ChatController : Controller
  {
    private readonly IChatService _service;
    private readonly ILogger _logger;

    public ChatController(IChatService service, ILogger logger) {
      _service = service;
      _logger = logger;
    }

    /// <summary>
    /// Endpoint responsável por criar um chat.
    /// </summary>
    /// <returns>Valida os dados passados para envio do sms e envia o mesmo para o numero de destino.</returns>
    [HttpPost("create")]
    [ProducesResponseType(typeof(Response<CreateChatResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<CreateChatResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Response<CreateChatResponse>), StatusCodes.Status417ExpectationFailed)]
    [ProducesResponseType(typeof(Response<CreateChatResponse>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Response<CreateChatResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Response<CreateChatResponse>>> CreateSession([FromBody] CreateChatRequest createChatRequest)
    {
      try
      {
        var token = Request.Headers["Authorization"];
        var response = await _service.CreateChatAsync(createChatRequest, token);
        return StatusCode(StatusCodes.Status200OK, new Response<CreateChatResponse>() { Status = 200, Message = $"Chat criado com sucesso.", Data = response, Success = true });
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "Exception while send sms!");
        switch (ex.Message)
        {
          case "errorInCreateChat":
            return StatusCode(StatusCodes.Status417ExpectationFailed, new Response<CreateChatResponse>() { Status = 403, Message = $"Não foi possível criar chat. {ex.Message}", Success = false });
            case "oneOfTheUsersDoesNotExist":
            return StatusCode(StatusCodes.Status417ExpectationFailed, new Response<CreateChatResponse>() { Status = 403, Message = $"Não foi possível criar chat. {ex.Message}: Algum dos membros informados para participar do chat não existe com o user_id informado.", Success = false });
          default:
            return StatusCode(StatusCodes.Status500InternalServerError, new Response<CreateChatResponse>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false });
        }   
      }
    }

    /// <summary>
    /// Endpoint responsável por criar um chat.
    /// </summary>
    /// <returns>Valida os dados passados para envio do sms e envia o mesmo para o numero de destino.</returns>
    [HttpGet("list")]
    [ProducesResponseType(typeof(Response<List<Chat>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<List<Chat>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Response<List<Chat>>), StatusCodes.Status417ExpectationFailed)]
    [ProducesResponseType(typeof(Response<List<Chat>>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Response<List<Chat>>), StatusCodes.Status500InternalServerError)]
    public ActionResult<Response<List<Chat>>> ListChatsWithOneMemberId([FromQuery] Guid member_id)
    {
      try
      {
        var response = _service.ListChatsWithOneMemberId(member_id);
        return StatusCode(StatusCodes.Status200OK, new Response<List<Chat>>() { Status = 200, Message = $"Chat retornado com sucesso.", Data = response, Success = true });
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "Exception while retrieve chat!");
        switch (ex.Message)
        {
          case "errorInCreateChat":
            return StatusCode(StatusCodes.Status417ExpectationFailed, new Response<List<Chat>>() { Status = 403, Message = $"Não foi possível retornar chat. {ex.Message}", Success = false });
          default:
            return StatusCode(StatusCodes.Status500InternalServerError, new Response<List<Chat>>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false });
        }   
      }
    }

    /// <summary>
    /// Endpoint responsável por atualizar um chat.
    /// </summary>
    /// <returns>Valida os dados passados para atualizaçãao do chat.</returns>
    [HttpPut("update")]
    [ProducesResponseType(typeof(Response<Chat>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<Chat>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Response<Chat>), StatusCodes.Status417ExpectationFailed)]
    [ProducesResponseType(typeof(Response<Chat>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Response<Chat>), StatusCodes.Status500InternalServerError)]
    public ActionResult<Response<Chat>> UpdateChat([FromBody] UpdateChatRequest updateChatRequest)
    {
      try
      {
        var response = _service.UpdateChat(updateChatRequest);
        return StatusCode(StatusCodes.Status200OK, new Response<Chat>() { Status = 200, Message = $"Chat atualizado com sucesso.", Data = response, Success = true });
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "Exception while retrieve chat!");
        switch (ex.Message)
        {
          case "errorInCreateChat":
            return StatusCode(StatusCodes.Status417ExpectationFailed, new Response<Chat>() { Status = 403, Message = $"Não foi possível retornar chat. {ex.Message}", Success = false });
          default:
            return StatusCode(StatusCodes.Status500InternalServerError, new Response<Chat>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false });
        }   
      }
    }
  }
}

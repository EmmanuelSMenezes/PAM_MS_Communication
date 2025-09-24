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
  [Route("notification")]
  [ApiController]
  [Authorize]
  public class NotificationController : Controller
  {
    private readonly INotificationService _service;
    private readonly ILogger _logger;

    public NotificationController(INotificationService service, ILogger logger) {
      _service = service;
      _logger = logger;
    }
    /// <summary>
    /// Endpoint responsável por criar uam notificação para um usuario especifico.
    /// </summary>
    /// <returns>Valida os dados passados para criação da notificação e retorna a notificação criada.</returns>
    [HttpPost("create")]
    [ProducesResponseType(typeof(Response<Notification>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<Notification>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Response<Notification>), StatusCodes.Status417ExpectationFailed)]
    [ProducesResponseType(typeof(Response<Notification>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Response<Notification>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Response<Notification>>> CreateNotification([FromBody] Notification notification)
    {
      try
      {
        var response = await _service.CreateNotification(notification);
        return StatusCode(StatusCodes.Status200OK, new Response<Notification>() { Status = 200, Message = $"Notificação criada com sucesso.", Data = response, Success = true });
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "Exception while create notification!");
        switch (ex.Message)
        {
          case "erroWhileCreateNotification":
            return StatusCode(StatusCodes.Status417ExpectationFailed, new Response<Notification>() { Status = 403, Message = $"Não foi possível criar notificação. {ex.Message}", Success = false });
          default:
            return StatusCode(StatusCodes.Status500InternalServerError, new Response<Notification>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false });
        }   
      }
    }

    /// <summary>
    /// Endpoint responsável por listar notificações para um usuario especifico.
    /// </summary>
    /// <returns>Valida os dados passados para listagem das notificações e retorna os dados.</returns>
    [HttpGet("listNotificationsByUserId")]
    [ProducesResponseType(typeof(Response<List<Notification>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<List<Notification>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Response<List<Notification>>), StatusCodes.Status417ExpectationFailed)]
    [ProducesResponseType(typeof(Response<List<Notification>>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Response<List<Notification>>), StatusCodes.Status500InternalServerError)]
    public  ActionResult<Response<List<Notification>>> ListNotificationsByUserId([FromQuery] Guid user_id)
    {
      try
      {
        var response = _service.ListNotificationsByUserId(user_id);
        return StatusCode(StatusCodes.Status200OK, new Response<List<Notification>>() { Status = 200, Message = $"Notificações retornada com sucesso.", Data = response, Success = true });
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "Exception while retrieve notifications!");
        switch (ex.Message)
        {
          default:
            return StatusCode(StatusCodes.Status500InternalServerError, new Response<List<Notification>>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false });
        }   
      }
    }

    /// <summary>
    /// Endpoint responsável por listar notificações para um usuario especifico.
    /// </summary>
    /// <returns>Valida os dados passados para listagem das notificações e retorna os dados.</returns>
    [HttpPut("markAsReadNotification")]
    [ProducesResponseType(typeof(Response<Notification>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<Notification>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Response<Notification>), StatusCodes.Status417ExpectationFailed)]
    [ProducesResponseType(typeof(Response<Notification>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Response<Notification>), StatusCodes.Status500InternalServerError)]
    public  ActionResult<Response<Notification>> MarkAsReadNotification([FromQuery] Guid notification_id)
    {
      try
      {
        var response = _service.MarkNotificationAsRead(notification_id);
        return StatusCode(StatusCodes.Status200OK, new Response<Notification>() { Status = 200, Message = $"Notificação atualizada com sucesso.", Data = response, Success = true });
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "Exception while update notification!");
        switch (ex.Message)
        {
          default:
            return StatusCode(StatusCodes.Status500InternalServerError, new Response<Notification>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false });
        }   
      }
    }
  }
}

using Serilog;
using Infrastructure.Repository;
using Domain.Model;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Application.Service
{
  public class NotificationService : INotificationService
  {
    private readonly INotificationRepository _repository;
    private readonly ILogger _logger;
    private readonly IHubContext<NotificationHub> _hubContext;
    public NotificationService(
      INotificationRepository repository,
      ILogger logger,
      IHubContext<NotificationHub> hubContext
    )
    {
      _repository = repository;
      _hubContext = hubContext;
      _logger = logger;
    }

    public async Task<Notification> CreateNotification(Notification notification)
    {
      try
      {
        var response = _repository.CreateNotification(notification);
        _logger.Information("[NotificationService - CreateNotification]: Notification created successfully.");
        await _hubContext.Clients.Group(response.user_id.ToString()).SendAsync("ReiceveNotifications", JsonConvert.SerializeObject(response));
        return response;
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "[NotificationService - CreateNotification]: Error while create notification.");
        throw ex;
      }
    }

    public List<Notification> ListNotificationsByUserId(Guid user_id)
    {
      try
      {
        var response = _repository.ListNotificationsByUserId(user_id);
        _logger.Information("[NotificationService - ListNotificationsByUserId]: Notifications retrieve successfully.");
        return response;
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "[NotificationService - ListNotificationsByUserId]: Error while retrieve notifications.");
        throw ex;
      }
    }

    public List<Notification> ListUnreadNotificationsByUserId(Guid user_id)
    {
      try
      {
        var response = _repository.ListNotificationsByUserId(user_id);
        _logger.Information("[NotificationService - ListUnreadNotificationsByUserId]: Notifications retieved successfully.");
        return response;
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "[NotificationService - ListUnreadNotificationsByUserId]: Error while retrieve notifications.");
        throw ex;
      }
    }

    public Notification MarkNotificationAsRead(Guid notification_id)
    {
      try
      {
        var response = _repository.MarkNotificationAsRead(notification_id);
        _logger.Information("[NotificationService - MarkNotificationAsRead]: Notification updated successfully.");
        return response;
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "[NotificationService - MarkNotificationAsRead]: Error while update notification.");
        throw ex;
      }
    }
  }
}

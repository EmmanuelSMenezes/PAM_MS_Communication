using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Domain.Model;
using Npgsql;
using Serilog;

namespace Infrastructure.Repository
{
  public class NotificationRepository : INotificationRepository
  {
    private readonly string _connectionString;
    private readonly ILogger _logger;
    public NotificationRepository(string connectionString, ILogger logger)
    {
      _connectionString = connectionString;
      _logger = logger;
    }

    public Notification CreateNotification(Notification notification)
    {
      try
      {
        var sql = $@"
          INSERT INTO communication.notification
          (title, description, user_id, type, aux_content, created_at)
          VALUES('{notification.title}', '{notification.description}', '{notification.user_id}', '{notification.type}', '{notification.aux_content}', CURRENT_TIMESTAMP)
          RETURNING *;
        ";
        using (var connection = new NpgsqlConnection(_connectionString))
        {
          var response = connection.Query<Notification>(sql).FirstOrDefault();
        _logger.Information("[NotificationRepository - CreateNotification]: Notification created successfully.");
          return response;
        }
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "[NotificationRepository - CreateNotification]: Error while create notification.");
        throw ex;
      }
    }

    public List<Notification> ListNotificationsByUserId(Guid user_id)
    {
      try
      {
        var sql = $@"SELECT * FROM communication.notification WHERE user_id = '{user_id}' ORDER BY created_at DESC;";
        using (var connection = new NpgsqlConnection(_connectionString))
        {
          var response = connection.Query<Notification>(sql).ToList();
          _logger.Information("[NotificationRepository - ListNotificationByUserId]: Notification retrieve successfully.");
          return response;
        }
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "[NotificationRepository - ListNotificationByUserId]: Error while list notifications by user_id.");
        throw ex;
      }
    }

    public List<Notification> ListUnreadNotificationsByUserId(Guid user_id)
    {
      try
      {
        var sql = $@"SELECT * FROM communication.notification WHERE user_id = '{user_id}' AND read_at IS NULL ORDER BY created_at DESC;";
        using (var connection = new NpgsqlConnection(_connectionString))
        {
          var response = connection.Query<Notification>(sql).ToList();
          _logger.Information("[NotificationRepository - ListUnreadNotificationsByUserId]: Notification retrieve successfully.");
          return response;
        }
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "[NotificationRepository - ListUnreadNotificationsByUserId]: Error while list notifications unread by user_id.");
        throw ex;
      }
    }

    public Notification MarkNotificationAsRead(Guid notification_id)
    {
      try
      {
        var sql = $@"UPDATE communication.notification SET read_at=CURRENT_TIMESTAMP WHERE notification_id ='{notification_id}' RETURNING *;";
        using (var connection = new NpgsqlConnection(_connectionString))
        {
          var response = connection.Query<Notification>(sql).FirstOrDefault();
          _logger.Information("[NotificationRepository - MarkNotificationAsRead]: Notification updated successfully.");
          return response;
        }
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "[NotificationRepository - MarkNotificationAsRead]: Error while update notification.");
        throw ex;
      }
    }
  }
}

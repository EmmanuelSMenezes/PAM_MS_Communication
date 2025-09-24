using System;
using System.Collections.Generic;
using Domain.Model;
using System.Threading.Tasks;

namespace Application.Service
{
  public interface INotificationService
  {
    Task<Notification> CreateNotification (Notification notification);
    List<Notification> ListNotificationsByUserId (Guid user_id);
    List<Notification> ListUnreadNotificationsByUserId (Guid user_id);
    Notification MarkNotificationAsRead (Guid notification_id);
  }
}

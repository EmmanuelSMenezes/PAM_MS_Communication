using System;
using System.Collections.Generic;
using Domain.Model;

namespace Infrastructure.Repository
{
  public interface INotificationRepository
  {
    Notification CreateNotification (Notification notification);
    List<Notification> ListNotificationsByUserId (Guid user_id);
    List<Notification> ListUnreadNotificationsByUserId (Guid user_id);
    Notification MarkNotificationAsRead (Guid notification_id);
  }
}
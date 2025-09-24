using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace Application.Service
{
  public class NotificationHub : Hub
  {
    private readonly IChatService _chatService;
    private readonly IMessageService _messageService;
    private readonly ILogger _logger;

    public NotificationHub(ILogger logger)
    {
      _logger = logger;
    }
    // private static List<UserStatus> _usersOnline = new List<UserStatus>();

    public async Task SendMessageToEspecificUser(Guid user_id, string notification)
    {
      await Clients.Group(user_id.ToString()).SendAsync("ReiceveNotifications", notification);
    }

    public async Task ListenNotificationsByUser(Guid user_id)
    {
      await Groups.AddToGroupAsync(Context.ConnectionId, user_id.ToString());
    }

    public async Task UnlistenNotificationsByUser(Guid user_id)
    {
      await Groups.RemoveFromGroupAsync(Context.ConnectionId, user_id.ToString());
    }
  }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Model;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Serilog;

namespace Application.Service
{
  public class ChatHub : Hub
  {
    private readonly IChatService _chatService;
    private readonly IMessageService _messageService;
    private readonly ILogger _logger;

    public ChatHub(IChatService chatService, IMessageService messageService, ILogger logger)
    {
      _chatService = chatService;
      _messageService = messageService;
      _logger = logger;
    }
    private static List<UserStatus> _usersOnline = new List<UserStatus>();

    public async Task RefreshStatus(Guid user_id, string status)
    {
      List<UserStatus> _newList = new List<UserStatus>();
      foreach (var uo in _usersOnline)
      {
        if (uo.User_id != user_id)
        {
          _newList.Add(uo); 
        }
      }
      _newList.Add(new UserStatus()
      {
        User_id = user_id,
        Status = status
      });
      _usersOnline = _newList;
      await Clients.All.SendAsync("RefreshStatus", JsonConvert.SerializeObject(_newList));
    }

    public async Task RefreshChatList(Guid user_id, string chatList) {
      await Clients.Group(user_id.ToString()).SendAsync("RefreshChatList", chatList);
    }

    public async Task JoinCommunicationChannel(Guid user_id)
    {
      await Groups.AddToGroupAsync(Context.ConnectionId, user_id.ToString());
    }

    public async Task SendMessageToEspecificChat(Guid chat_id, Guid user_id, string message, string token)
    {
      await Clients.Group(chat_id.ToString()).SendAsync("ReceiveMessage", user_id.ToString(), message);
      var messageJson = JsonConvert.DeserializeObject<CreateMessageRequest>(message);
      var response = _messageService.CreateMessage(messageJson, token);
    }

    public async Task JoinChat(Guid chat_id)
    {
      await Groups.AddToGroupAsync(Context.ConnectionId, chat_id.ToString());
    }

    public async Task LeaveChat(Guid chat_id)
    {
      await Groups.RemoveFromGroupAsync(Context.ConnectionId, chat_id.ToString());
    }
  }
}
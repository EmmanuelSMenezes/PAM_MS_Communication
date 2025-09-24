using System;
using Domain.Model;
using Serilog;
using Infrastructure.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Collections.Generic;

namespace Application.Service
{
  public class MessageService : IMessageService
  {
    private readonly IMessageRepository _repository;
    private readonly INotificationService _notificationService;
    private readonly IChatService _chatService;
    private readonly string _secret;
    private readonly ILogger _logger;
    public MessageService(
      IMessageRepository repository,
      INotificationService notificationService,
      IChatService chatService,
      string secret,
      ILogger logger
    )
    {
      _logger = logger;
      _repository = repository;
      _notificationService = notificationService;
      _chatService = chatService;
      _secret = secret;
    }

    public CreateMessageResponse CreateMessage(CreateMessageRequest createMessageRequest, string token)
    {
      try
      {
        if (String.IsNullOrEmpty(token)) throw new ArgumentException("tokenIsNotProvided");
        var decodedToken = GetDecodeToken(token.Split(' ')[1], _secret);
        createMessageRequest.Created_by = decodedToken.UserId;

        var messageType = GetMessageTypeByTag(createMessageRequest.MessageTypeTag);
        createMessageRequest.MessageType = messageType.Message_type_id;

        var response = _repository.CreateMessage(createMessageRequest);
        _logger.Information("[MessageService - CreateMessage]: Message created succesfully.");

        var chat = _chatService.GetChatById(createMessageRequest.Chat_id);
        List<Guid> sendNotificationMembers = new List<Guid>();

        var senderName = "";
        foreach (var member in chat.MembersProfile)
        {
          if (member.User_id == response.Sender_id && chat.MembersProfile.Count == 2) {
            senderName = member.Name;
          }
        }
        foreach (var member in chat.MembersProfile)
        {
          if (member.User_id != response.Sender_id) {
            _notificationService.CreateNotification(new Notification() {
              type = "chat_message",
              aux_content = createMessageRequest.Chat_id.ToString(),
              title = $"{senderName} enviou uma mensagem.",
              user_id = member.User_id,
              description = response.Content,
              created_at = DateTime.Now
            });
          }
        }
        
        return new CreateMessageResponse()
        {
          Content = response.Content,
          Created_at = response.Created_at,
          Message_id = response.Message_id,
          MessageType = response.MessageType,
          Read_at = response.Read_at,
          Sender_id = response.Sender_id
        };
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "[MessageService - CreateMessage]: Error while create message.");
        throw ex;
      }
    }

    public DecodedToken GetDecodeToken(string token, string secret)
    {
      DecodedToken decodedToken = new DecodedToken();
      JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
      JwtSecurityToken jwtSecurityToken = jwtSecurityTokenHandler.ReadToken(token) as JwtSecurityToken;
      foreach (Claim claim in jwtSecurityToken.Claims)
      {
        if (claim.Type == "email")
        {
          decodedToken.email = claim.Value;
        }
        else if (claim.Type == "name")
        {
          decodedToken.name = claim.Value;
        }
        else if (claim.Type == "userId")
        {
          decodedToken.UserId = new Guid(claim.Value);
        }
        else if (claim.Type == "roleId")
        {
          decodedToken.RoleId = new Guid(claim.Value);
        }

      }
      return decodedToken;

      throw new Exception("invalidToken");
    }

    public MessageType GetMessageTypeByTag(string tag)
    {
      try
      {
        return _repository.GetMessageTypeByTag(tag);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public Message UpdateMessage(UpdateMessageRequest updateMessageRequest)
    {
      try
      {
        return _repository.UpdateMessage(updateMessageRequest);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }
  }
}

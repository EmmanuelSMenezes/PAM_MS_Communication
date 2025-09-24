using System;
using Domain.Model;
using Serilog;
using Infrastructure.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Application.Service
{
  public class ChatService : IChatService
  {
    private readonly IChatRepository _repository;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly string _secret;
    private readonly ILogger _logger;
    public ChatService(
      IChatRepository repository,
      IHubContext<ChatHub> hubContext,
      string secret,
      ILogger logger
    )
    {
      _logger = logger;
      _hubContext = hubContext;
      _repository = repository;
      _secret = secret;
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

    public async Task<CreateChatResponse> CreateChatAsync(CreateChatRequest createChatRequest, string token)
    {
      try
      {
        var decodedToken = GetDecodeToken(token.Split(' ')[1], _secret);
        createChatRequest.Created_by = decodedToken.UserId;
        var response = _repository.CreateChat(createChatRequest);
        foreach (var member in response.Members)
        {
          await _hubContext.Clients.Group(member.ToString()).SendAsync("RefreshChatList", JsonConvert.SerializeObject(response));
        _logger.Information("[ChatService - CreateChat]: Chat SignalR Send successfully.");
        }
        _logger.Information("[ChatService - CreateChat]: Chat created successfully.");
        return new CreateChatResponse() {
          Chat_id = response.Chat_id,
          Created_at = response.Created_at,
          Created_by = response.Created_by,
          Description = response.Description,
          UnReadCountMessages = response.UnReadCountMessages,
          Updated_at = response.Updated_at,
          Members = response.Members,
          MembersProfile = response.MembersProfile,
          Closed = response.Closed,
          Closed_by = response.Closed_by,
          LastMessage = response.LastMessage,
          Messages = response.Messages,
          Order_id = response?.Order_id
        };
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "[ChatService - CreateChat]: Error while create chat.");
        throw ex;
      }
    }

    public List<Chat> ListChatsWithOneMemberId(Guid member_id)
    {
      try
      {
        var response = _repository.ListChatsWithOneMemberId(member_id);
        _logger.Information("[ChatService - ListChatsWithOneMemberId]: Chat returned successfully.");
        return response;
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "[ChatService - ListChatsWithOneMemberId]: Error while retrieve chat.");
        throw ex;
      }
    }

    public List<Guid> ListUserIdsAvaliable()
    {
      try
      {
        var response = _repository.ListUserIdsAvaliable();
        _logger.Information("[ChatService - ListUserIdsAvaliable]: Users list returned successfully.");
        return response;
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "[ChatService - ListUserIdsAvaliable]: Error while list users avaliable.");
        throw ex;
      }
    }

    public Chat UpdateChat(UpdateChatRequest updateChatRequest)
    {
      try
      {
        var response = _repository.UpdateChat(updateChatRequest);
        _logger.Information("[ChatService - UpdateChat]: Chat updated successfully.");
        return response;
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "[ChatService - UpdateChat]: Error while update chat.");
        throw ex;
      }
    }

    public Chat GetChatById(Guid chat_id)
    {
      try
      {
        var response = _repository.GetChatById(chat_id);
        _logger.Information("[ChatService - GetChatById]: Chat returned successfully.");
        return response;
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "[ChatService - GetChatById]: Error while retrieve chat.");
        throw ex;
      }
    }
  }
}

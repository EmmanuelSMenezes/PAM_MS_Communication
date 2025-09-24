using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Model;

namespace Application.Service
{
  public interface IChatService
  {
    Task<CreateChatResponse> CreateChatAsync(CreateChatRequest createChatRequest, string token);
    Chat UpdateChat(UpdateChatRequest updateChatRequest);
    DecodedToken GetDecodeToken(string token, string secret);
    List<Chat> ListChatsWithOneMemberId(Guid member_id);
    List<Guid> ListUserIdsAvaliable();
    Chat GetChatById (Guid chat_id);
  }
}

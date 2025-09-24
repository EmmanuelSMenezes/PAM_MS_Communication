using System;
using System.Collections.Generic;
using Domain.Model;

namespace Infrastructure.Repository
{
  public interface IChatRepository
  {
    Chat CreateChat(CreateChatRequest createChatRequest);
    Chat UpdateChat(UpdateChatRequest updateChatRequest);
    List<Chat> ListChatsWithOneMemberId(Guid member_id);
    List<Guid> ListUserIdsAvaliable();
    Chat GetChatById (Guid chat_id);
  }
}

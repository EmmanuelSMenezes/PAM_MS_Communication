using System;
using System.Collections.Generic;
using Domain.Model;

namespace Infrastructure.Repository
{
  public interface IMessageRepository
  {
    Message CreateMessage(CreateMessageRequest createMessageRequest);
    Message UpdateMessage(UpdateMessageRequest updateMessageRequest);

    MessageType GetMessageTypeByTag(string tag);
    List<Message> ListMessagesByChatId(Guid chat_id);
  }
}

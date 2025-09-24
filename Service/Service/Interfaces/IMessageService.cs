using Domain.Model;

namespace Application.Service
{
  public interface IMessageService
  {
    CreateMessageResponse CreateMessage(CreateMessageRequest createMessageRequest, string token);
    MessageType GetMessageTypeByTag(string tag);
    Message UpdateMessage(UpdateMessageRequest updateMessageRequest);
    DecodedToken GetDecodeToken(string token, string secret);
  }
}

using System;

namespace Domain.Model
{
  public class SendMessageRequest
  {
    public Guid? Chat_id { get; set; }
    public Guid User_id { get; set; }
    public Guid Target_user { get; set; }
    public string Content { get; set; }
    public string MessageType { get; set; }
    public string Token { get; set; }
  }
}

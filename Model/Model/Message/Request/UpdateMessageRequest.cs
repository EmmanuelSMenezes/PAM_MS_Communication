using System;

namespace Domain.Model
{
  public class UpdateMessageRequest: Message
  {
    public new DateTime Read_at { get; set; }
    public new Guid Message_id { get; set; }
    public Guid Chat_id { get; set; }
  }
}
using System;

namespace Domain.Model
{
  public class UpdateChatRequest
  {
    public Guid Closed_by { get; set; }
    public Guid Chat_id { get; set; }
  }
}

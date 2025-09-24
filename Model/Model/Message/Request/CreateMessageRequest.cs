using System;

namespace Domain.Model
{
  public class CreateMessageRequest
  {
    public string Content { get; set; }
    public DateTime? Read_at { get; set; }
    public DateTime Created_at { get; set; }
    public Guid Sender_id { get; set; }
    public Guid Chat_id { get; set; }
    // [JsonIgnore]
    public Guid? Created_by { get; set; }
    public Guid? MessageType { get; set; }
    public string MessageTypeTag { get; set; }
  }
}
using System;

namespace Domain.Model
{
  public class MessageType
  {
    public Guid Message_type_id { get; set; }
    public string Description { get; set; }
    public string Tag { get; set; }
  }
}
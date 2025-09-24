using System;

namespace Domain.Model
{

  public class Notification
  {
    public Guid notification_id { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public DateTime? read_at { get; set; }
    public Guid user_id { get; set; }
    public string type { get; set; }
    public string aux_content { get; set; }
    public DateTime created_at { get; set; }
  }
}
using Newtonsoft.Json;

namespace Domain.Model
{
  public class SendEmailRequest
  {
    [JsonIgnore]
    public ulong DeliveryTag { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public string Email { get; set; }
  }
}

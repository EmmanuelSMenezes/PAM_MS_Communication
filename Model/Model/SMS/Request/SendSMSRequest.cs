using Newtonsoft.Json;

namespace Domain.Model
{
  public class SendSMSRequest
  {
    public string Body { get; set; }
    [JsonIgnore]
    public string FromPhoneNumber { get; set; }
    [JsonIgnore]
    public ulong DeliveryTag { get; set; }
    public string ToPhoneNumber { get; set; }
  }
}

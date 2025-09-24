using System;
using Newtonsoft.Json;

namespace Domain.Model
{
  public class Message
  {
    [JsonProperty("content")]
    public string Content { get; set; }
    [JsonProperty("read_at")]
    public DateTime? Read_at { get; set; }
    [JsonProperty("created_at")]
    public DateTime Created_at { get; set; }
    [JsonProperty("sender_id")]
    public Guid Sender_id { get; set; }
    [JsonIgnore]
    [JsonProperty("message_id")]
    public Guid Message_id { get; set; }
    [JsonProperty("messageType")]
    public string MessageType { get; set; }
    [JsonIgnore]
    [JsonProperty("token")]
    public string Token { get; set; }
  }
}
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Domain.Model
{
  public class Chat
  {
    [JsonProperty("chat_id")]
    public Guid Chat_id { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("created_at")]
    public DateTime? Created_at { get; set; }
    [JsonProperty("updated_at")]
    public DateTime? Updated_at { get; set; }
    [JsonProperty("created_by")]
    public Guid Created_by { get; set; }
    [JsonProperty("members")]
    public Guid[] Members { get; set; }
    [JsonProperty("membersProfile")]
    public List<Member> MembersProfile { get; set; } = new List<Member>();
    [JsonProperty("lastMessage")]
    public Message LastMessage { get; set; }
    [JsonProperty("messages")]
    public List<Message> Messages { get; set; }
    [JsonProperty("unReadCountMessages")]
    public int UnReadCountMessages { get; set; }
    [JsonProperty("closed_by")]
    public Guid? Closed_by { get; set; }
    [JsonProperty("closed")]
    public DateTime? Closed { get; set; }
    [JsonProperty("order_id")]
    public Guid? Order_id { get; set; }
  }
}
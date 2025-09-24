using System;
using Newtonsoft.Json;

namespace Domain.Model
{
  public class Member
  {
    [JsonProperty("user_id")]
    public Guid User_id { get; set; }
    [JsonProperty("avatar")]
    public string Avatar { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    
  }
}
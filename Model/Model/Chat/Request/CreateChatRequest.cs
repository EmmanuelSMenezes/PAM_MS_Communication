using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Domain.Model
{
  public class CreateChatRequest
  {
    public Guid? Order_id { get; set; }
    public string Description { get; set; }
    [JsonIgnore]
    public Guid? Created_by { get; set; }
    public List<Guid> Members { get; set; }
  }
}

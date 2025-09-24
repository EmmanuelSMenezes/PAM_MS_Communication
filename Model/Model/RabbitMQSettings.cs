using System.Collections.Generic;

public class RabbitMQSettings
{
    public string Uri { get; set; }
    public Exchange EmailExchange { get; set; }
    public Exchange SMSExchange { get; set; }
    public Queue SendMailQueue { get; set; }
    public Queue SendSMSQueue { get; set; }
}

public class Exchange {
  public string Name { get; set; }
  public string Type { get; set; }
  public bool Durable { get; set; } = true;
  public bool Autodelete { get; set; } = false;
  public IDictionary<string, object> Arguments { get; set; } = null;
}

public class Queue {
  public string QueueName { get; set; }
  public string ExchangeName { get; set; }
  public string RoutingKey { get; set; }
  public string Description { get; set; }
  public bool Durable { get; set; }
  public bool Exclusive { get; set; }
  public bool AutoDelete { get; set; }
  public int? TTLms { get; set; }
  public IDictionary<string, object> Arguments { get; set; }
}
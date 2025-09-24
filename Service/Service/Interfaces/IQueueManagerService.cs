using RabbitMQ.Client;
using System;
using System.Threading.Tasks;

namespace Application.Service
{
  public interface IQueueManagerService
  {
    Task<bool> Publish(object request, string queueName, IBasicProperties properties = null);
    void Consumer<T>(Action<T> procedure, string queueName, bool autoAck = true);
  }
}

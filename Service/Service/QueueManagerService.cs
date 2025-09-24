using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Serilog;
using RabbitMQ.Client.Events;
using Domain.Model;

namespace Application.Service
{
  public class QueueManagerService : IQueueManagerService
  {

    private IModel _channel;
    private readonly RabbitMQSettings _rabbitMQSettings;
    private readonly ILogger _logger;
    public readonly Guid _id = new Guid();
    public readonly IConnection _rabbitmqConnection;
    public QueueManagerService(
      IModel channel,
      RabbitMQSettings rabbitMQSettings,
      ILogger logger,
      IConnection rabbitmqConnection
      )
    {
      _channel = channel;
      _rabbitMQSettings = rabbitMQSettings;
      _logger = logger;
      _rabbitmqConnection = rabbitmqConnection;
    }

    public async Task<bool> Publish(object request, string queueName, IBasicProperties properties = null)
    {
      return await Task.Run(() =>
      {
        try
        {
          if (_channel == null || !_channel.IsOpen) {
            _channel = _rabbitmqConnection.CreateModel();
          }

          string serializedRequest = JsonConvert.SerializeObject(request, Newtonsoft.Json.Formatting.Indented);

          var body = Encoding.UTF8.GetBytes(serializedRequest);

          if (properties == null) properties = _channel.CreateBasicProperties();
          properties.Persistent = true;

          _channel.ConfirmSelect();

          Queue queue = new Queue();
          if (queueName == _rabbitMQSettings.SendMailQueue.QueueName)
          {
            queue = _rabbitMQSettings.SendMailQueue;
          }
          if (queueName == _rabbitMQSettings.SendSMSQueue.QueueName)
          {
            queue = _rabbitMQSettings.SendSMSQueue;
          }

          _channel.BasicPublish(queue.ExchangeName, queue.RoutingKey, properties, body);

          _channel.WaitForConfirmsOrDie();

          _logger.Information("[RabbitMQ.Publisher] Message published");

          return true;
        }
        catch (Exception ex)
        {
          _logger.Error(ex, "[RabbitMQ.Publisher] Message not published");
          throw ex;
        }
      });
    }

    public void Consumer<T>(Action<T> procedure, string queueName, bool autoAck)
    {
      if (_channel == null || !_channel.IsOpen) {
        _channel = _rabbitmqConnection.CreateModel();
      }

      Queue queue = new Queue();
      var consumer = new EventingBasicConsumer(_channel);

      consumer.Received += async (model, ea) =>
      {
        try
        {
          var message = Encoding.UTF8.GetString(ea.Body.ToArray());

          _logger.Information($"\n\n[CONSUMER] Message: {message}\n\n");

          dynamic JsonMessage = JsonConvert.DeserializeObject<T>(message);
          JsonMessage.DeliveryTag = ea.DeliveryTag;

          procedure.Invoke(JsonMessage);
        }
        catch (Exception ex)
        {
          _logger.Error(ex, $"[CONSUMER] Erro ao pegar mensagem reenfileirada\n");
        }
      };
      if (queueName == _rabbitMQSettings.SendMailQueue.QueueName)
      {
        queue = _rabbitMQSettings.SendMailQueue;
      }
      if (queueName == _rabbitMQSettings.SendSMSQueue.QueueName)
      {
        queue = _rabbitMQSettings.SendSMSQueue;
      }

      _channel.BasicConsume(queue.QueueName, autoAck, consumer);

    }
  }
}
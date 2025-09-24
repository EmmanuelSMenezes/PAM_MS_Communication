using System;
using System.Threading.Tasks;
using Domain.Model;
using Serilog;

namespace Application.Service
{
  public class ConsumerService : IConsumerService
  {
    private readonly ILogger _logger;
    private readonly RabbitMQSettings _rabbitMQSettings;
    private readonly IQueueManagerService _queueManagerService;
    private readonly IEmailService _emailService;
    private readonly ISMSService _smsService;
    public ConsumerService(
      IQueueManagerService queueManagerService,
      IEmailService emailService,
      ISMSService smsService,
      ILogger logger,
      RabbitMQSettings rabbitMQSettings
    )
    {
      _queueManagerService = queueManagerService;
      _rabbitMQSettings = rabbitMQSettings;
      _emailService = emailService;
      _smsService = smsService;
      _logger = logger;
    }

    public async Task StartConsumerEmailQueue()
    {
      try
      {
        _queueManagerService.Consumer<SendEmailRequest>((emailMessage) => _emailService.SendMail(emailMessage), _rabbitMQSettings.SendMailQueue.QueueName, false);

        await Task.Delay(5000);

        _logger.Information("[Worker - StartConsumerEmailQueue] Consumer subscribed...");
      }
      catch (Exception ex)
      {
        _logger.Error($"[Worker - StartConsumerEmailQueue] Error on start consumer. \n {ex}");
      }
    }

    public async Task StartConsumerSMSQueue()
    {
      try
      {
        _queueManagerService.Consumer<SendSMSRequest>((smsMessage) => _smsService.SendSMS(smsMessage), _rabbitMQSettings.SendSMSQueue.QueueName, false);

        await Task.Delay(5000);

        _logger.Information("[Worker - StartConsumerSMSQueue] Consumer subscribed...");
      }
      catch (Exception ex)
      {
        _logger.Error($"[Worker - StartConsumerSMSQueue] Error on start consumer. \n {ex}");
      }
    }
  }
}

using System;
using System.Threading.Tasks;
using Domain.Model;
using RabbitMQ.Client;
using Serilog;
using Twilio.Rest.Api.V2010.Account;

namespace Application.Service
{
  public class SMSService : ISMSService
  {
    private readonly TwilioSettings _twilioSettings;
    private readonly ILogger _logger;
    private readonly IModel _channel;
    public SMSService(
      TwilioSettings twilioSettings,
      ILogger logger,
      IModel channel
    )
    {
      _twilioSettings = twilioSettings;
      _logger = logger;
      _channel = channel;
    }

    public async Task<bool> SendSMS(SendSMSRequest sendSMSRequest) {
      try
      {
        var message = await MessageResource.CreateAsync(
          body: sendSMSRequest.Body,
          from: new Twilio.Types.PhoneNumber(_twilioSettings.PhoneNumber),
          to: new Twilio.Types.PhoneNumber(sendSMSRequest.ToPhoneNumber)
        );

        if (message.ErrorCode != null) throw new Exception("errorInSMSSent");
        _logger.Information("[SMSService] - SMS enviado com sucesso.");
        _channel.BasicAck(sendSMSRequest.DeliveryTag, false);
        return true;
      }
      catch (Exception ex)
      {
        _channel.BasicNack(sendSMSRequest.DeliveryTag, false, true);
        _logger.Error(ex, "[SMSService] - Erro ao enviar SMS.");
        throw ex;
      }
    }
  }
}

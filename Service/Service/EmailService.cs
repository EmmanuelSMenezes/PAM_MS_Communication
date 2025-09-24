using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Domain.Model;
using Serilog;
using RabbitMQ.Client;

namespace Application.Service
{
  public class EmailService : IEmailService
  {
    private readonly ILogger _logger;
    private readonly IModel _channel;
    private readonly EmailSettings _emailSettings;
    public EmailService(
      ILogger logger,
      EmailSettings emailSettings,
      IModel channel
    )
    {
      _logger = logger;
      _emailSettings = emailSettings;
      _channel = channel;
    }

    public async Task<bool> SendMail(SendEmailRequest sendEmailRequest)
    {
      try
      {
        MailMessage mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(_emailSettings.FromEmail);
        mailMessage.To.Add(new MailAddress(sendEmailRequest.Email));

        mailMessage.Subject = sendEmailRequest.Subject;
        mailMessage.Body = sendEmailRequest.Body;
        mailMessage.IsBodyHtml = true;

        using (var smtp = new SmtpClient())
        {
          System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
          smtp.Host = _emailSettings.PrimaryDomain;
          smtp.Port = int.Parse(_emailSettings.PrimaryPort);
          smtp.EnableSsl = Convert.ToBoolean(_emailSettings.EnableSsl);
          smtp.UseDefaultCredentials = Convert.ToBoolean(_emailSettings.UseDefaultCredentials);
          smtp.Credentials = new NetworkCredential(_emailSettings.FromEmail, _emailSettings.UsernamePassword);

          await smtp.SendMailAsync(mailMessage);
        }
        _logger.Information("[EmailService] - Email enviado com sucesso.");
        _channel.BasicAck(sendEmailRequest.DeliveryTag, false);
        return true;
      }
      catch (Exception ex)
      {
        _channel.BasicNack(sendEmailRequest.DeliveryTag, false, true);
        _logger.Error(ex, "[EmailService] - Erro ao enviar email.");
        throw ex;
      }
    }
  }
}

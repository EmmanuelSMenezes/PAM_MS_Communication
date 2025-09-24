using Application.Service;
using Domain.Model;
using FluentValidation.AspNetCore;
using FluentValidation;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using Serilog;
using System.IO;
using System.Linq;
using System.Text;
using Twilio;
using RabbitMQ.Client;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using RabbitMQ.Client.Events;
using WebApi.Extensions;
using System;

namespace MS_Communication
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {

      services.AddMvc(option => option.EnableEndpointRouting = false);
      services.AddControllers().AddFluentValidation();
      services.AddControllers();
      services.AddSignalR();
      services.AddCors();
      services.AddLogging();
      services.AddWorker(Configuration);
      services.AddMSCommunication(Configuration);

      var key = Encoding.ASCII.GetBytes(Configuration.GetSection("MSCommunicationSettings").GetSection("PrivateSecretKey").Value);
      services.AddAuthentication(x =>
      {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(x =>
      {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateIssuer = false,
          ValidateAudience = false
        };
      });

      // Add framework services.

      services.AddSwaggerDocument(config =>
      {
        config.PostProcess = document =>
              {
                document.Info.Version = "V1";
                document.Info.Title = "PAM - Microservice Communication";
                document.Info.Description = "API's Documentation of Microservice Communication of PAM Plataform";
              };

        config.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
        {
          Type = OpenApiSecuritySchemeType.ApiKey,
          Name = "Authorization",
          In = OpenApiSecurityApiKeyLocation.Header,
        });

        config.OperationProcessors.Add(
                  new AspNetCoreOperationSecurityScopeProcessor("JWT"));
      });

      string logFilePath = Configuration.GetSection("LogSettings").GetSection("LogFilePath").Value;
      string logFileName = Configuration.GetSection("LogSettings").GetSection("LogFileName").Value;

      string connectionString = Configuration.GetSection("MSCommunicationSettings").GetSection("ConnectionString").Value;
      string privateSecretKey = Configuration.GetSection("MSCommunicationSettings").GetSection("PrivateSecretKey").Value;
      string tokenValidationMinutes = Configuration.GetSection("MSCommunicationSettings").GetSection("TokenValidationMinutes").Value;

      BaseURLWebApplication baseURLWebApplication = new BaseURLWebApplication()
      {
        Administrator = Configuration.GetSection("MSCommunicationSettings").GetSection("baseURLWebApplication:Administrator").Value,
        Partner = Configuration.GetSection("MSCommunicationSettings").GetSection("baseURLWebApplication:Partner").Value
      };

      TwilioSettings twilioSettings = new TwilioSettings()
      {
        AccountSID = Configuration.GetSection("TwilioAccount").GetSection("AccountSID").Value,
        AuthToken = Configuration.GetSection("TwilioAccount").GetSection("AuthToken").Value,
        PhoneNumber = Configuration.GetSection("TwilioAccount").GetSection("PhoneNumber").Value,
      };

      EmailSettings emailSettings = new EmailSettings()
      {
        PrimaryDomain = Configuration.GetSection("EmailSettings:PrimaryDomain").Value,
        PrimaryPort = Configuration.GetSection("EmailSettings:PrimaryPort").Value,
        UsernameEmail = Configuration.GetSection("EmailSettings:UsernameEmail").Value,
        UsernamePassword = Configuration.GetSection("EmailSettings:UsernamePassword").Value,
        FromEmail = Configuration.GetSection("EmailSettings:FromEmail").Value,
        ToEmail = Configuration.GetSection("EmailSettings:ToEmail").Value,
        CcEmail = Configuration.GetSection("EmailSettings:CcEmail").Value,
        EnableSsl = Configuration.GetSection("EmailSettings:EnableSsl").Value,
        UseDefaultCredentials = Configuration.GetSection("EmailSettings:UseDefaultCredentials").Value
      };

      RabbitMQSettings rabbitMQSettings = new RabbitMQSettings()
      {
        Uri = Configuration.GetSection("RabbitMQSettings:Uri").Value,
        EmailExchange = new Exchange()
        {
          Name = Configuration.GetSection("RabbitMQSettings:EmailExchange").GetSection("Name").Value,
          Type = Configuration.GetSection("RabbitMQSettings:EmailExchange").GetSection("Type").Value
        },
        SMSExchange = new Exchange()
        {
          Name = Configuration.GetSection("RabbitMQSettings:SMSExchange").GetSection("Name").Value,
          Type = Configuration.GetSection("RabbitMQSettings:SMSExchange").GetSection("Type").Value
        },
        SendMailQueue = new Queue()
        {
          ExchangeName = Configuration.GetSection("RabbitMQSettings:SendMailQueue").GetSection("ExchangeName").Value,
          QueueName = Configuration.GetSection("RabbitMQSettings:SendMailQueue").GetSection("QueueName").Value,
          RoutingKey = Configuration.GetSection("RabbitMQSettings:SendMailQueue").GetSection("RoutingKey").Value
        },
        SendSMSQueue = new Queue()
        {
          ExchangeName = Configuration.GetSection("RabbitMQSettings:SendSMSQueue").GetSection("ExchangeName").Value,
          QueueName = Configuration.GetSection("RabbitMQSettings:SendSMSQueue").GetSection("QueueName").Value,
          RoutingKey = Configuration.GetSection("RabbitMQSettings:SendSMSQueue").GetSection("RoutingKey").Value
        }
      };

      services.AddSingleton((ILogger)new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.File(Path.Combine(logFilePath, logFileName), rollingInterval: RollingInterval.Day)
        .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
        .CreateLogger());

      var rabbitMQConnection = new ConnectionFactory()
      {
        Uri = new Uri(rabbitMQSettings.Uri),
      }.CreateConnection();
      var rabbitMQChannel = rabbitMQConnection.CreateModel();

      try
      {
        rabbitMQChannel.ExchangeDeclare(
            rabbitMQSettings.EmailExchange.Name,
            rabbitMQSettings.EmailExchange.Type,
            rabbitMQSettings.EmailExchange.Durable,
            rabbitMQSettings.EmailExchange.Autodelete,
            rabbitMQSettings.EmailExchange.Arguments
        );
        rabbitMQChannel.ExchangeDeclare(
            rabbitMQSettings.SMSExchange.Name,
            rabbitMQSettings.SMSExchange.Type,
            rabbitMQSettings.SMSExchange.Durable,
            rabbitMQSettings.SMSExchange.Autodelete,
            rabbitMQSettings.SMSExchange.Arguments
        );
        rabbitMQChannel.QueueDeclare(
            rabbitMQSettings.SendMailQueue.QueueName,
            rabbitMQSettings.SendMailQueue.Durable,
            rabbitMQSettings.SendMailQueue.Exclusive,
            rabbitMQSettings.SendMailQueue.AutoDelete,
            rabbitMQSettings.SendMailQueue.Arguments
        );
        rabbitMQChannel.QueueBind(
            rabbitMQSettings.SendMailQueue.QueueName,
            rabbitMQSettings.SendMailQueue.ExchangeName,
            rabbitMQSettings.SendMailQueue.RoutingKey
        );
        rabbitMQChannel.QueueDeclare(
            rabbitMQSettings.SendSMSQueue.QueueName,
            rabbitMQSettings.SendSMSQueue.Durable,
            rabbitMQSettings.SendSMSQueue.Exclusive,
            rabbitMQSettings.SendSMSQueue.AutoDelete,
            rabbitMQSettings.SendSMSQueue.Arguments
        );
        rabbitMQChannel.QueueBind(
            rabbitMQSettings.SendSMSQueue.QueueName,
            rabbitMQSettings.SendSMSQueue.ExchangeName,
            rabbitMQSettings.SendSMSQueue.RoutingKey
        );
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }

      TwilioClient.Init(twilioSettings.AccountSID, twilioSettings.AuthToken);

      services.AddSignalR();

      services.AddSingleton<IChatRepository, ChatRepository>(provider => new ChatRepository(connectionString, provider.GetService<ILogger>()));
      services.AddSingleton<INotificationRepository, NotificationRepository>(provider => new NotificationRepository(connectionString, provider.GetService<ILogger>()));
      services.AddSingleton<IMessageRepository, MessageRepository>(provider => new MessageRepository(connectionString, provider.GetService<ILogger>()));
      services.AddSingleton<IChatService, ChatService>(provider => new ChatService(provider.GetService<IChatRepository>(), provider.GetService<IHubContext<ChatHub>>(), privateSecretKey, provider.GetService<ILogger>()));
      services.AddSingleton<INotificationService, NotificationService>(provider => new NotificationService(provider.GetService<INotificationRepository>(), provider.GetService<ILogger>(), provider.GetService<IHubContext<NotificationHub>>()));
      services.AddSingleton<IMessageService, MessageService>(provider => new MessageService(
        provider.GetService<IMessageRepository>(),
        provider.GetService<INotificationService>(),
        provider.GetService<IChatService>(),
        privateSecretKey,
        provider.GetService<ILogger>()
        ));
      services.AddSingleton<ISMSService, SMSService>(provider => new SMSService(twilioSettings, provider.GetService<ILogger>(), rabbitMQChannel));
      services.AddSingleton<IEmailService, EmailService>(provider => new EmailService(provider.GetService<ILogger>(), emailSettings, rabbitMQChannel));
      services.AddSingleton<IQueueManagerService, QueueManagerService>(provider => new QueueManagerService(rabbitMQChannel, rabbitMQSettings, provider.GetService<ILogger>(), rabbitMQConnection));

      services.AddTransient<IConsumerService, ConsumerService>(
        provider => new ConsumerService(
          provider.GetService<IQueueManagerService>(),
          provider.GetService<IEmailService>(),
          provider.GetService<ISMSService>(),
          provider.GetService<ILogger>(),
          rabbitMQSettings
        )
      );

      services.AddTransient<IValidator<SendSMSRequest>, SendSMSRequestValidator>();
      services.AddTransient<IValidator<SendEmailRequest>, SendEmailRequestValidator>();


    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseOpenApi();
      
      app.UseSwaggerUi3();

      app.UseAuthentication();

      app.UseRouting();

      app.UseAuthorization();

      app.UseCors(builder => builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        endpoints.MapHub<ChatHub>("/chat-hub");
        endpoints.MapHub<NotificationHub>("/notification-hub");
        endpoints.MapHub<CoreHub>("/core-hub");
      });
      
      app.UseMvc();
    }
  }
}
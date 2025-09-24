using Domain.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Application.Service
{
  public class WorkerService : WorkerBase
  {
    private IConsumerService _consumerService;
    private new Microsoft.Extensions.Logging.ILogger _logger;

    public WorkerService(
      ILogger<WorkerService> logger,
      IOptions<WorkerSettings> config,
      IConsumerService consumerService)
        : base(config.Value.DefaultExecutionInterval, logger)
    {
      _logger = logger;
      config = null;
      _consumerService = consumerService;
    }

    protected override async Task RunAsync(object state)
    {
      await _consumerService.StartConsumerEmailQueue();
      await _consumerService.StartConsumerSMSQueue();
    }
  }
}

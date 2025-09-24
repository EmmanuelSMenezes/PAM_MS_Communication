using Domain.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Application.Service
{
  public class WorkerServiceWithInterval : WorkerBase
  {
    private IConsumerService _consumerService;
    private new Microsoft.Extensions.Logging.ILogger _logger;

    public WorkerServiceWithInterval(
      ILogger<WorkerService> logger,
      IOptions<WorkerSettings> config,
      IConsumerService consumerService)
        : base(config.Value.DefaultExecutionInterval + 30, logger)
    {
      _logger = logger;
      config = null;
      _consumerService = consumerService;
    }

    protected override Task RunAsync(object state)
    {
      return Task.CompletedTask;
    }
  }
}

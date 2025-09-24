using System.Threading.Tasks;

namespace Application.Service
{
  public interface IConsumerService
  {
    Task StartConsumerSMSQueue();
    Task StartConsumerEmailQueue();
  }
}

using System.Threading.Tasks;
using Domain.Model;

namespace Application.Service
{
  public interface ISMSService
  {
    Task<bool> SendSMS(SendSMSRequest sendSMSRequest);
  }
}

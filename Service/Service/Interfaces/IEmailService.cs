using System.Threading.Tasks;
using Domain.Model;

namespace Application.Service
{
  public interface IEmailService
  {
    Task<bool> SendMail(SendEmailRequest sendEmailRequest);
  }
}

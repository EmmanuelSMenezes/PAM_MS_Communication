using Serilog;

namespace Infrastructure.Repository
{
  public class SMSRepository : ISMSRepository
  {
    private readonly string _connectionString;
    private readonly ILogger _logger;
    public SMSRepository(string connectionString, ILogger logger)
    {
      _connectionString = connectionString;
      _logger = logger;
    }

  
  }
}

using Serilog;

namespace Infrastructure.Repository
{
  public class EmailRepository : IEmailRepository
  {
    private readonly string _connectionString;
    private readonly ILogger _logger;
    public EmailRepository(string connectionString, ILogger logger)
    {
      _connectionString = connectionString;
      _logger = logger;
    }

  }
}

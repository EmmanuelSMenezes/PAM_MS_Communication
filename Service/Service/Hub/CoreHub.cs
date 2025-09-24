using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace Application.Service
{
  public class CoreHub : Hub
  {
    private readonly ILogger _logger;

    public CoreHub(ILogger logger)
    {
      _logger = logger;
    }

    public async Task DisconnectUser(Guid user_id)
    {
      await Clients.All.SendAsync("DisconnectUser", user_id.ToString());
    }

    public async Task RefreshProductListOnPartner()
    {
      await Clients.All.SendAsync("RefreshProductListOnPartner", "true");
    }
  }
}
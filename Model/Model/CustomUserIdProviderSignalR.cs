using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

public class CustomUserIdProviderSignalR : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        var httpCtx = connection.GetHttpContext();
        var user_id = httpCtx.Request.Query["access_token"].ToString();
        return user_id;
    }

    private string ObterCustomUserId(ClaimsPrincipal user)
    {
        return user.FindFirstValue("CustomUserIdClaim");
    }
}

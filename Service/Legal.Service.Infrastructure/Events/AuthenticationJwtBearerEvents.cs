using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Legal.Service.Infrastructure.Events;

public static class AuthenticationJwtBearerEvents
{
    public delegate Task JwtBearerEventHandler(MessageReceivedContext messageReceivedContext);

    public static event JwtBearerEventHandler JwtBearerEvent;

    public static async Task CallJwtBearerEvent(MessageReceivedContext messageReceivedContext)
    {
        await JwtBearerEvent?.Invoke(messageReceivedContext);
    }
}
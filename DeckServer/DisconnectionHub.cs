using Microsoft.AspNetCore.SignalR;

namespace DeckServer;
public class DisconnectionHub : Hub
{
    private readonly ILogger<DisconnectionHub> logger;
    private HttpContext? httpContext;

    public DisconnectionHub(ILogger<DisconnectionHub> logger)
    {
        this.logger = logger;   
    }
    
    private HttpContext GetHttpContext()
    {
        httpContext ??= Context.GetHttpContext() ?? throw new ArgumentNullException(
            nameof(httpContext),
            "Hubs cannot be used outside of a HttpContext"
        );
        return httpContext;
    }

    public override async Task OnConnectedAsync()
    {
        logger.LogInformation("Hub connect: {id} [{wid}]", Context.ConnectionId, GetHttpContext().Session.Id);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if(exception is not null) logger.LogWarning(exception.Message);
        logger.LogInformation("Hub disconnect: {id} [{wid}]", Context.ConnectionId, GetHttpContext().Session.Id);
        logger.LogInformation("Cleaning up orphaned memory");
        #if DEBUG
        logger.LogDebug("Cleaning up {id}", httpContext?.Session.Id);
        #endif
        WidgetTagHelper.Clean(httpContext?.Session.Id ?? "");
        logger.LogInformation("{count} currently connected", WidgetTagHelper.ConnectionCount());
        logger.LogInformation("Loaded widgets: {count}", WidgetTagHelper.WidgetCount());

        await base.OnDisconnectedAsync(exception);
    }
}
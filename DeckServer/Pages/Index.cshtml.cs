using System.Text;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeckServer.Pages;

public class IndexModel : PageModel, IDisposable
{
    private readonly ILogger<IndexModel> logger;

    // IDE0290: Use primary constructor
    // Unavailable in this version of .NET SDK; linter still corrects me
    #pragma warning disable IDE0290
    public IndexModel(ILogger<IndexModel> logger)
    {
        this.logger = logger;
    }
    #pragma warning restore IDE0290

    public void OnGet()
    {
        logger.LogInformation(
            "Connection from {rip} [{id}]; {count} currently connected",
            HttpContext.Connection.RemoteIpAddress,
            HttpContext.Connection.Id,
            WidgetTagHelper.ConnectionCount()
        );
        #if DEBUG
        logger.LogInformation("Loaded widgets: {count}", WidgetTagHelper.WidgetCount());
        #endif
    }

    public void Dispose()
    {
        #if DEBUG
        logger.LogDebug("Disposing of connection {id}", HttpContext.Connection.Id);
        #endif
        GC.SuppressFinalize(this);
    }
}

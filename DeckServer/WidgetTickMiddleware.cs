using System.Data;
using System.Text;
using DeckUtils;

namespace DeckServer;
public class WidgetTickMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger logger;

    // IDE0290: Use primary constructor
    // Unavailable in this version of .NET SDK; linter still corrects me
    #pragma warning disable IDE0290
    public WidgetTickMiddleware(RequestDelegate next, ILogger<WidgetTickMiddleware> logger)
    #pragma warning restore IDE0290
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {   
        if(context.Request.Method == "POST" && context.Request.Path == "/api/obsdeck/v1/tick")
        {
            #if DEBUG
            logger.LogDebug("Tick request from {rip}", context.Connection.RemoteIpAddress);
            #endif
            string id = context.Connection.Id;
            if(WidgetTagHelper.Widgets.TryGetValue(id, out List<Widget>? wl))
            {
                
                foreach(Widget w in wl)
                {
                    string upd = "";
                    try
                    {
                        upd = w.OnUpdate();
                    }
                    catch(InvalidOperationException ex)
                    {
                        logger.LogCritical("Epic fail while ticking widget {w}: {msg}", w, ex.Message);
                        logger.LogCritical(Utils.CRIT_TRACE_LOG, ex.StackTrace);
                        logger.LogCritical(Utils.REPORT_URL, "https://github.com/thatmightyguy/obsdeck/issues");
                    }
                    await context.Response.Body.WriteAsync(
                        new ReadOnlyMemory<byte>(
                            Encoding.UTF8.GetBytes(
                                $"{w.Id};{upd}|"
                            )
                        )
                    );
                }
            }
            else
            {
                logger.LogError("Widget tick requested; no widgets registered for client {id}", id);
            }
        }
        else
        {
            await next(context);
        }
    }
}
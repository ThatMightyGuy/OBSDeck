using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DeckServer;

[HtmlTargetElement("widget")]
public class WidgetTagHelper : TagHelper
{
    private const string ERR_WIDGET_PLACEHOLDER = "<p class=\"widget error\">A widget has failed to load</p>";

    [ViewContext]
    public ViewContext? ViewContext {get; set;}
    public string? Src {get; set;}

    // IDE0028: Collection initialization can be simplified
    // I am not shortening new() to []
    #pragma warning disable IDE0028
    public readonly static Dictionary<string, List<Widget>> Widgets = new();
    #pragma warning restore IDE0028
    private static readonly object widgetsLock = new();

    private string? ConnectionId => ViewContext?.HttpContext.Session.Id;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (!string.IsNullOrEmpty(Src))
        {
            string src = File.ReadAllText(Src);
            string htmlContent = ERR_WIDGET_PLACEHOLDER;

            try
            {
                Widget w = new(src);
                htmlContent = w.OnUpdate();
                if(GetWidgetsForConnection(
                    ConnectionId ?? throw new ArgumentNullException(
                        nameof(ViewContext), 
                        "Widgets cannot be used outside of a ViewContext")
                    ).Count == 0
                )
                {
                    lock(widgetsLock)
                    {
                        #pragma warning disable IDE0028
                        Widgets.Add(ConnectionId, new());
                        #pragma warning restore IDE0028
                    }
                }
                lock(widgetsLock)
                    Widgets[ConnectionId].Add(w);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Widget {Src} failed to load: {ex.Message}");
            }

            output.TagName = null;
            output.Content.SetHtmlContent(htmlContent);
        }
    }

    public static bool Clean(string id)
    {
        List<Widget> widgets = GetWidgetsForConnection(id);
        lock(widgetsLock)
        {
            foreach(Widget w in widgets)
                w.Dispose();
            return Widgets.Remove(id);
        }

    }
    public static int ConnectionCount()
    {
        lock(widgetsLock)
            return Widgets.Count;
    }
    public static int WidgetCount()
    {
        int widgets = 0;
        lock(widgetsLock)
            foreach(List<Widget> w in Widgets.Values)
                widgets += w.Count;
        return widgets;
    }
    public static List<Widget> GetWidgetsForConnection(string id)
    {
        try
        {
            lock (widgetsLock)
                return Widgets[id];
        }
        catch(KeyNotFoundException)
        {
            return new();
        }
    }
}
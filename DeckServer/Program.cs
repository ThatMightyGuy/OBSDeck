namespace DeckServer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddRazorPages();
        builder.Services.AddSignalR();
        var app = builder.Build();
        
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseMiddleware<WidgetTickMiddleware>();
        app.MapRazorPages();
        app.MapHub<DisconnectionHub>("/api/obsdock/v1/disconnect");
        app.Run();
    }
}

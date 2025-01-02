using Conesoft.Hosting;
using Conesoft.Plugin.NotificationService;
using Conesoft.Plugin.NotificationService.Notifiers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddHostConfigurationFiles()
    .AddHostEnvironmentInfo()
    .AddLoggingService()
    ;

builder.Services.AddTransient<Storage>();
builder.Services.AddTransient<INotifier, WirepusherNotifier>();
builder.Services.AddTransient<INotifier, LocalNotifier>();
builder.Services.AddTransient<INotifier, LogNotifier>();
builder.Services.AddHostedService<NotificationWatcher>();

builder.Services.AddHttpClient();

var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(app.Services.GetRequiredService<Storage>().Content.Path),
    ServeUnknownFileTypes = true,
    RequestPath = "/content"
});

app.MapGet("/", () => "Hello, Notifications");

app.Run();
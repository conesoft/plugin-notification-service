using Conesoft.Hosting;
using Conesoft.Plugin.NotificationService.Components;
using Conesoft.Plugin.NotificationService.Features.ContentStorage;
using Conesoft.Plugin.NotificationService.Features.Notifications.Implementations;
using Conesoft.Plugin.NotificationService.Features.Notifications.Interfaces;
using Conesoft.Plugin.NotificationService.Features.Notifications.Services;
using Conesoft.PwaGenerator;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddHostConfigurationFiles()
    .AddHostEnvironmentInfo()
    .AddLoggingService()
    .AddUsersWithStorage()
    ;

builder.Services
    .AddHttpClient()
    .AddTransient<Storage>()
    .AddTransient<INotifier, WirepusherNotifier>()
    .AddTransient<INotifier, LocalNotifier>()
    .AddTransient<INotifier, LogNotifier>()
    .AddHostedService<NotificationWatcher>()
    .AddRazorComponents()
    ;

var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(app.Services.GetRequiredService<Storage>().Content.Path),
    ServeUnknownFileTypes = true,
    RequestPath = "/content"
});
app.MapPwaInformationFromAppSettings();
app.MapStaticAssets();
app.MapUsersWithStorage();
app.MapRazorComponents<App>();

app.Run();
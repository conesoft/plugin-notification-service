using Conesoft.Plugin.NotificationService.Features.Notifications.Content;
using Conesoft.Plugin.NotificationService.Features.Notifications.Interfaces;
using Serilog;

namespace Conesoft.Plugin.NotificationService.Features.Notifications.Implementations;

class LogNotifier : INotifier
{
    void INotifier.Show(Notification notification)
    {
        Log.Information("notification {@notification}", new
        {
            notification.Title,
            notification.Message,
            notification.Url,
            Image = notification.Image != null
        });
    }
}
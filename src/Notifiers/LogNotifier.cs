using Serilog;

namespace Conesoft.Plugin.NotificationService.Notifiers;

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
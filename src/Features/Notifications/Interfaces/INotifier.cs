using Conesoft.Plugin.NotificationService.Features.Notifications.Content;

namespace Conesoft.Plugin.NotificationService.Features.Notifications.Interfaces;

interface INotifier
{
    void Show(Notification notification);
}
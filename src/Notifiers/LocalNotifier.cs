using Microsoft.Toolkit.Uwp.Notifications;
using System;

namespace Conesoft.Plugin.NotificationService.Notifiers;

class LocalNotifier : INotifier
{
    void INotifier.Show(Notification notification)
    {
        var builder = new ToastContentBuilder();
        builder.AddText(notification.Title, AdaptiveTextStyle.Title);
        builder.AddText(notification.Message);

        if (notification.Image != null)
        {
            builder.AddHeroImage(new Uri(notification.Image.Path, UriKind.Absolute));
        }

        if (notification.Url != null)
        {
            builder.AddToastActivationInfo(notification.Url, ToastActivationType.Protocol);
        }

        builder.Show();
    }
}
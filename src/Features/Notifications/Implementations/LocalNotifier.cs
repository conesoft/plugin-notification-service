using Conesoft.Plugin.NotificationService.Features.Notifications.Content;
using Conesoft.Plugin.NotificationService.Features.Notifications.Interfaces;
using Microsoft.Toolkit.Uwp.Notifications;
using System;

namespace Conesoft.Plugin.NotificationService.Features.Notifications.Implementations;

class LocalNotifier : INotifier
{
    void INotifier.Show(Notification notification)
    {
        if (notification.To == null || notification.To == "Family" || notification.To == "davepermen" || notification.To == "Admin")
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
}
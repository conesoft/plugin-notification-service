using Conesoft.Files;
using Conesoft.Hosting;
using Conesoft.Plugin.NotificationService.Features.Notifications.Content;
using Conesoft.Plugin.NotificationService.Features.Notifications.Interfaces;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Conesoft.Plugin.NotificationService.Features.Notifications.Implementations;

class WirepusherNotifier(IHttpClientFactory factory, IConfiguration configuration, HostEnvironment environment) : INotifier
{
    void INotifier.Show(Notification notification)
    {
        var client = factory.CreateClient();

        var wirepusher = configuration["wirepusher:base"]!;
        var wirepusherids = GetWirepusherIdsFor(notification) ?? [configuration["wirepusher:secret"]!];

        foreach (var wirepusherid in wirepusherids)
        {
            var wirepusherNotification = new QueryBuilder
            {
                { "title", notification.Title ?? "Conesoft Notification" },
                { "message", notification.Message },
                { "type", "Server" },
                { "id", wirepusherid }
            };

            if (notification.Url != null)
            {
                wirepusherNotification.Add("action", notification.Url);
            }

            if (notification.Image != null)
            {
                wirepusherNotification.Add("image_url", $"https://notifications.conesoft.net/content/{notification.Image.Name}");
            }

            client.GetAsync(wirepusher + wirepusherNotification.ToQueryString());
        }
    }

    string[]? GetWirepusherIdsFor(Notification notification)
    {
        if (notification.To == null)
        {
            return null;
        }

        HashSet<string> notifications = [];
        var users = (environment.Global.Storage / "Users").Directories;
        foreach (var user in users)
        {
            var notificationFile = user / Filename.From("notifications", "txt");
            var userConfiguration = user / Filename.From("login-data", "json");
            if (notificationFile.Exists && notificationFile.Now.ReadText() is string notificationSecret)
            {
                if (notification.To == user.Name)
                {
                    notifications.Add(notificationSecret);
                }
                if (userConfiguration.Now.ReadFromJson<UserData>() is UserData userData && userData.Roles.Contains(notification.To))
                {
                    notifications.Add(notificationSecret);
                }
            }
        }
        return [.. notifications];
    }
    record UserData(string[] Roles);
}
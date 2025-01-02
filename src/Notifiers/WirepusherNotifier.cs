using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace Conesoft.Plugin.NotificationService.Notifiers;

class WirepusherNotifier(IHttpClientFactory factory, IConfiguration configuration) : INotifier
{
    void INotifier.Show(Notification notification)
    {
        var client = factory.CreateClient();

        var wirepusher = configuration["wirepusher:base"]!;
        var wirepusherSecret = configuration["wirepusher:secret"]!;

        var wirepusherNotification = new QueryBuilder
        {
            { "title", notification.Title ?? "Conesoft Notification" },
            { "message", notification.Message },
            { "type", "Server" },
            { "id", wirepusherSecret }
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
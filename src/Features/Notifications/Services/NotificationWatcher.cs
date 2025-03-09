using Conesoft.Files;
using Conesoft.Plugin.NotificationService.Features.ContentStorage;
using Conesoft.Plugin.NotificationService.Features.Notifications.Content;
using Conesoft.Plugin.NotificationService.Features.Notifications.Interfaces;
using FolkerKinzel.DataUrls;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Conesoft.Plugin.NotificationService.Features.Notifications.Services;

class NotificationWatcher(Storage storage, IEnumerable<INotifier> notifiers) : IHostedService
{
    CancellationTokenSource? cts;
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var notificationsDirectory = storage.Notifications;
        var contentDirectory = storage.Content;

        var last = DateTime.MinValue;

        cts = notificationsDirectory.Live(() =>
        {
            while (notificationsDirectory.FilteredFiles("*.json", allDirectories: false).OrderByDescending(e => e.Info.LastWriteTimeUtc).FirstOrDefault() is File file)
            {
                if (last != file.Info.CreationTime)
                {
                    last = file.Info.CreationTime;
                    Log.Information("notification from " + file.Name);

                    if (file.WhenReady.Now.ReadFromJson<NotificationFromJson>() is NotificationFromJson content)
                    {
                        File? image = null;
                        if (DataUrl.TryGetBytes(content.DataUrlEncodedImage, out var bytes, out var extension) && bytes != null && extension != null)
                        {
                            image = contentDirectory / Filename.From(file.NameWithoutExtension, extension);
                            image.Now.WriteBytes(bytes);
                        }

                        var notification = new Notification(
                            content.Title,
                            content.Message,
                            Image: image,
                            content.Url,
                            content.To
                        );

                        foreach (var notifier in notifiers)
                        {
                            notifier.Show(notification);
                        }
                    }

                    file.WhenReady.Now.Delete();
                }
            }
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => cts?.CancelAsync() ?? Task.CompletedTask;
}

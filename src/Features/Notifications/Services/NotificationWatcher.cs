using Conesoft.Files;
using Conesoft.Plugin.NotificationService.Features.ContentStorage;
using Conesoft.Plugin.NotificationService.Features.Notifications.Content;
using Conesoft.Plugin.NotificationService.Features.Notifications.Interfaces;
using FolkerKinzel.DataUrls;
using Microsoft.Extensions.Hosting;
using Serilog;
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

        bool processing = false;

        cts = notificationsDirectory.Live(async () =>
        {
            if(processing == false)
            {
                processing = true;
                var filtered = notificationsDirectory.FilteredFiles("*.json", allDirectories: false);
                while (filtered.Any())
                {
                    if (notificationsDirectory.FilteredFiles("*.json", allDirectories: false).OrderByDescending(e => e.Info.LastWriteTimeUtc).FirstOrDefault() is File file)
                    {
                        Log.Information("notification from " + file.Name);

                        if (await file.WhenReady.ReadFromJson<NotificationFromJson>() is NotificationFromJson content)
                        {
                            File? image = null;
                            if (DataUrl.TryGetBytes(content.DataUrlEncodedImage, out var bytes, out var extension) && bytes != null && extension != null)
                            {
                                image = contentDirectory / Filename.From(file.NameWithoutExtension, extension);
                                await image.WriteBytes(bytes);
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

                        await file.WhenReady.Delete();
                    }
                }
            }
            processing = false;
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => cts?.CancelAsync() ?? Task.CompletedTask;
}

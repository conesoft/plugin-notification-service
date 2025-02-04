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

class NotificationWatcher(Storage storage, IEnumerable<INotifier> notifiers) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var notificationsDirectory = storage.Notifications;
        var contentDirectory = storage.Content;

        try
        {
            await foreach (var _ in notificationsDirectory.Live(cancellation: stoppingToken))
            {
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
                                content.Url
                            );

                            foreach (var notifier in notifiers)
                            {
                                notifier.Show(notification);
                            }
                        }

                        file.WhenReady.Delete();

                        while (file.Exists)
                        {
                            await Task.Delay(10, stoppingToken);
                        }
                    }
                }
            }
        }
        catch (Exception exception)
        {
            Log.Error("Exception: {exception}", exception);
        }
    }
}

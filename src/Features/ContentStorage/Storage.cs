﻿using Conesoft.Files;
using Conesoft.Hosting;

namespace Conesoft.Plugin.NotificationService.Features.ContentStorage;

class Storage
{
    private readonly HostEnvironment environment;

    public Storage(HostEnvironment environment)
    {
        this.environment = environment;

        Notifications.Create();
        Content.Create();
    }

    public Directory Notifications => environment.Global.Storage / "plugins" / "notifications";
    public Directory Content => Notifications / "content";
}
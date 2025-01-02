using Conesoft.Hosting;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddHostConfigurationFiles()
    .AddHostEnvironmentInfo()
    .AddLoggingService()
    ;

var host = builder.Build();

host.MapGet("/", () => "Hello, Notifications");

host.Run();
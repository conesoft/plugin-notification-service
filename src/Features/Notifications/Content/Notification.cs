using Conesoft.Files;

namespace Conesoft.Plugin.NotificationService.Features.Notifications.Content;

record Notification(string Title, string Message, File? Image, string? Url);
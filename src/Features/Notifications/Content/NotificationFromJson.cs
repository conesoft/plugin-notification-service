using System.Text.Json.Serialization;

namespace Conesoft.Plugin.NotificationService.Features.Notifications.Content;

record NotificationFromJson(string Title, string Message, [property: JsonPropertyName("image")] string? DataUrlEncodedImage, string? Url);
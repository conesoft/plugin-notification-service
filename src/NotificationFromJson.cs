using System.Text.Json.Serialization;

namespace Conesoft.Plugin.NotificationService;

record NotificationFromJson(string Title, string Message, [property:JsonPropertyName("image")] string? DataUrlEncodedImage, string? Url);
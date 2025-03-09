using System.Text.Json.Serialization;

namespace Conesoft.Plugin.NotificationService.Features.Notifications.Content;

record NotificationFromJson(string Title, string Message)
{
    [JsonPropertyName("image")]
    public string? DataUrlEncodedImage { get; set; }

    public string? Url { get; set; }
    public string? To { get; set; }
}
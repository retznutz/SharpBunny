using System.Text.Json.Serialization;

namespace SharpBunny.Models;

public class PurgeUrlsRequest
{
    [JsonPropertyName("urls")]
    public List<string> Urls { get; set; } = new();

    [JsonPropertyName("async")]
    public bool Async { get; set; } = false;
}

public class PurgeHistoryEntry
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("dateCreated")]
    public DateTime DateCreated { get; set; }

    [JsonPropertyName("dateCompleted")]
    public DateTime? DateCompleted { get; set; }

    [JsonPropertyName("pullZoneId")]
    public int? PullZoneId { get; set; }

    [JsonPropertyName("pullZoneName")]
    public string PullZoneName { get; set; } = string.Empty;
}
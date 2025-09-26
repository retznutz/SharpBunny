using System.Text.Json.Serialization;

namespace SharpBunny.Models;

public class VideoPlayData
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("views")]
    public long Views { get; set; }

    [JsonPropertyName("watchTime")]
    public long WatchTime { get; set; }

    [JsonPropertyName("impressions")]
    public long Impressions { get; set; }
}
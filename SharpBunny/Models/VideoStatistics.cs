using System.Text.Json.Serialization;

namespace SharpBunny.Models;

public class VideoStatistics
{
    [JsonPropertyName("totalViews")]
    public long TotalViews { get; set; }

    [JsonPropertyName("totalWatchTime")]
    public long TotalWatchTime { get; set; }

    [JsonPropertyName("averageWatchTime")]
    public double AverageWatchTime { get; set; }

    [JsonPropertyName("impressions")]
    public long Impressions { get; set; }

    [JsonPropertyName("clicks")]
    public long Clicks { get; set; }

    [JsonPropertyName("clickThroughRate")]
    public double ClickThroughRate { get; set; }

    [JsonPropertyName("countries")]
    public Dictionary<string, long> Countries { get; set; } = new();

    [JsonPropertyName("devices")]
    public Dictionary<string, long> Devices { get; set; } = new();

    [JsonPropertyName("referrers")]
    public Dictionary<string, long> Referrers { get; set; } = new();
}
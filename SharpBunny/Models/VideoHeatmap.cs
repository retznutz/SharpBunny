using System.Text.Json.Serialization;

namespace SharpBunny.Models;

public class VideoHeatmap
{
    [JsonPropertyName("timeStamp")]
    public double TimeStamp { get; set; }

    [JsonPropertyName("watchers")]
    public int Watchers { get; set; }
}

public class VideoHeatmapData
{
    [JsonPropertyName("heatmap")]
    public List<VideoHeatmap> Heatmap { get; set; } = new();
}
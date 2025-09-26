using System.Text.Json.Serialization;

namespace SharpBunny.Models;

public class VideoResolution
{
    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("bitrate")]
    public int Bitrate { get; set; }

    [JsonPropertyName("framerate")]
    public double Framerate { get; set; }

    [JsonPropertyName("fileSize")]
    public long FileSize { get; set; }

    [JsonPropertyName("fileName")]
    public string FileName { get; set; } = string.Empty;
}
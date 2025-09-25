using System.Text.Json.Serialization;

namespace SharpBunny.Models;

public class Collection
{
    [JsonPropertyName("videoLibraryId")]
    public int VideoLibraryId { get; set; }

    [JsonPropertyName("guid")]
    public string Guid { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("videoCount")]
    public int VideoCount { get; set; }

    [JsonPropertyName("totalSize")]
    public long TotalSize { get; set; }

    [JsonPropertyName("previewVideoIds")]
    public string PreviewVideoIds { get; set; } = string.Empty;
}
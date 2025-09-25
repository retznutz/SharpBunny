using System.Text.Json.Serialization;

namespace SharpBunny.Models;

public class Video
{
    [JsonPropertyName("videoLibraryId")]
    public int VideoLibraryId { get; set; }

    [JsonPropertyName("guid")]
    public string Guid { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("dateUploaded")]
    public DateTime DateUploaded { get; set; }

    [JsonPropertyName("views")]
    public long Views { get; set; }

    [JsonPropertyName("isPublic")]
    public bool IsPublic { get; set; }

    [JsonPropertyName("length")]
    public int Length { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("framerate")]
    public double Framerate { get; set; }

    [JsonPropertyName("rotation")]
    public int Rotation { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("availableResolutions")]
    public string AvailableResolutions { get; set; } = string.Empty;

    [JsonPropertyName("thumbnailCount")]
    public int ThumbnailCount { get; set; }

    [JsonPropertyName("encodeProgress")]
    public int EncodeProgress { get; set; }

    [JsonPropertyName("storageSize")]
    public long StorageSize { get; set; }

    [JsonPropertyName("captions")]
    public List<Caption> Captions { get; set; } = new();

    [JsonPropertyName("hasMP4Fallback")]
    public bool HasMP4Fallback { get; set; }

    [JsonPropertyName("collectionId")]
    public string? CollectionId { get; set; }

    [JsonPropertyName("thumbnailFileName")]
    public string ThumbnailFileName { get; set; } = string.Empty;

    [JsonPropertyName("averageWatchTime")]
    public int AverageWatchTime { get; set; }

    [JsonPropertyName("totalWatchTime")]
    public long TotalWatchTime { get; set; }

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("chapters")]
    public List<Chapter> Chapters { get; set; } = new();

    [JsonPropertyName("moments")]
    public List<Moment> Moments { get; set; } = new();

    [JsonPropertyName("metaTags")]
    public List<MetaTag> MetaTags { get; set; } = new();

    [JsonPropertyName("transcodingMessages")]
    public List<TranscodingMessage> TranscodingMessages { get; set; } = new();
}

public class Caption
{
    [JsonPropertyName("srclang")]
    public string SourceLanguage { get; set; } = string.Empty;

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("captionsFileName")]
    public string CaptionsFileName { get; set; } = string.Empty;
}

public class Chapter
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("start")]
    public int Start { get; set; }

    [JsonPropertyName("end")]
    public int End { get; set; }
}

public class Moment
{
    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public int Timestamp { get; set; }
}

public class MetaTag
{
    [JsonPropertyName("property")]
    public string Property { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}

public class TranscodingMessage
{
    [JsonPropertyName("timeStamp")]
    public DateTime TimeStamp { get; set; }

    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("issueCode")]
    public int IssueCode { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}
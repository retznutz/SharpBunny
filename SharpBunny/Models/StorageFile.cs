using System.Text.Json.Serialization;

namespace SharpBunny.Models;

public class StorageFile
{
    [JsonPropertyName("ArrayNumber")]
    public int ArrayNumber { get; set; }

    [JsonPropertyName("Checksum")]
    public string? Checksum { get; set; }

    [JsonPropertyName("ContentType")]
    public string ContentType { get; set; } = string.Empty;

    [JsonPropertyName("DateCreated")]
    public string DateCreated { get; set; } = string.Empty;

    [JsonPropertyName("Guid")]
    public string Guid { get; set; } = string.Empty;

    [JsonPropertyName("IsDirectory")]
    public bool IsDirectory { get; set; }

    [JsonPropertyName("LastChanged")]
    public string LastChanged { get; set; } = string.Empty;

    [JsonPropertyName("Length")]
    public long Length { get; set; }

    [JsonPropertyName("ObjectName")]
    public string ObjectName { get; set; } = string.Empty;

    [JsonPropertyName("Path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("ReplicatedZones")]
    public string ReplicatedZones { get; set; } = string.Empty;

    [JsonPropertyName("ServerId")]
    public int ServerId { get; set; }

    [JsonPropertyName("StorageZoneId")]
    public int StorageZoneId { get; set; }

    [JsonPropertyName("StorageZoneName")]
    public string StorageZoneName { get; set; } = string.Empty;

    [JsonPropertyName("UserId")]
    public string UserId { get; set; } = string.Empty;
}
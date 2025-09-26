using System.Text.Json.Serialization;

namespace SharpBunny.Models;

public class Country
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("continentCode")]
    public string ContinentCode { get; set; } = string.Empty;

    [JsonPropertyName("isEU")]
    public bool IsEU { get; set; }

    [JsonPropertyName("taxRate")]
    public decimal TaxRate { get; set; }
}
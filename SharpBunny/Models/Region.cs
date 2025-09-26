using System.Text.Json.Serialization;

namespace SharpBunny.Models;

public class Region
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("regionCode")]
    public string RegionCode { get; set; } = string.Empty;

    [JsonPropertyName("continentCode")]
    public string ContinentCode { get; set; } = string.Empty;

    [JsonPropertyName("countryCode")]
    public string CountryCode { get; set; } = string.Empty;

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("priceTier")]
    public int PriceTier { get; set; }

    [JsonPropertyName("regionPrice")]
    public decimal RegionPrice { get; set; }
}
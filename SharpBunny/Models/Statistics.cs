using System.Text.Json.Serialization;

namespace SharpBunny.Models;

public class PullZoneStatistics
{
    [JsonPropertyName("totalBandwidthUsed")]
    public long TotalBandwidthUsed { get; set; }

    [JsonPropertyName("totalRequestsServed")]
    public long TotalRequestsServed { get; set; }

    [JsonPropertyName("cacheHitRate")]
    public double CacheHitRate { get; set; }

    [JsonPropertyName("bandwidthUsedChart")]
    public Dictionary<string, long> BandwidthUsedChart { get; set; } = new();

    [JsonPropertyName("bandwidthCachedChart")]
    public Dictionary<string, long> BandwidthCachedChart { get; set; } = new();

    [JsonPropertyName("cacheHitRateChart")]
    public Dictionary<string, double> CacheHitRateChart { get; set; } = new();

    [JsonPropertyName("requestsServedChart")]
    public Dictionary<string, long> RequestsServedChart { get; set; } = new();

    [JsonPropertyName("pullRequestsPulledChart")]
    public Dictionary<string, long> PullRequestsPulledChart { get; set; } = new();

    [JsonPropertyName("userBalanceHistoryChart")]
    public Dictionary<string, decimal> UserBalanceHistoryChart { get; set; } = new();

    [JsonPropertyName("userStorageUsedChart")]
    public Dictionary<string, long> UserStorageUsedChart { get; set; } = new();

    [JsonPropertyName("geoTrafficDistribution")]
    public Dictionary<string, long> GeoTrafficDistribution { get; set; } = new();

    [JsonPropertyName("top3FileTypeChart")]
    public Dictionary<string, long> Top3FileTypeChart { get; set; } = new();

    [JsonPropertyName("top3StatusCodeChart")]
    public Dictionary<string, long> Top3StatusCodeChart { get; set; } = new();

    [JsonPropertyName("error4xxChart")]
    public Dictionary<string, long> Error4xxChart { get; set; } = new();

    [JsonPropertyName("error5xxChart")]
    public Dictionary<string, long> Error5xxChart { get; set; } = new();
}

public class BillingStatistics
{
    [JsonPropertyName("totalBandwidthUsed")]
    public long TotalBandwidthUsed { get; set; }

    [JsonPropertyName("totalRequestsServed")]
    public long TotalRequestsServed { get; set; }

    [JsonPropertyName("totalStorageUsed")]
    public long TotalStorageUsed { get; set; }

    [JsonPropertyName("totalCost")]
    public decimal TotalCost { get; set; }

    [JsonPropertyName("balance")]
    public decimal Balance { get; set; }

    [JsonPropertyName("thisMonthCharges")]
    public decimal ThisMonthCharges { get; set; }

    [JsonPropertyName("bandwidthUsedChart")]
    public Dictionary<string, long> BandwidthUsedChart { get; set; } = new();

    [JsonPropertyName("requestsServedChart")]
    public Dictionary<string, long> RequestsServedChart { get; set; } = new();

    [JsonPropertyName("storageUsedChart")]
    public Dictionary<string, long> StorageUsedChart { get; set; } = new();

    [JsonPropertyName("costChart")]
    public Dictionary<string, decimal> CostChart { get; set; } = new();

    [JsonPropertyName("balanceChart")]
    public Dictionary<string, decimal> BalanceChart { get; set; } = new();
}

public class CountryStatistics
{
    [JsonPropertyName("countryCode")]
    public string CountryCode { get; set; } = string.Empty;

    [JsonPropertyName("totalBandwidthUsed")]
    public long TotalBandwidthUsed { get; set; }

    [JsonPropertyName("totalRequestsServed")]
    public long TotalRequestsServed { get; set; }

    [JsonPropertyName("cacheHitRate")]
    public double CacheHitRate { get; set; }
}
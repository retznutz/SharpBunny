using System.Net.Http.Headers;
using SharpBunny.Countries;
using SharpBunny.DnsZones;
using SharpBunny.PullZones;
using SharpBunny.Regions;
using SharpBunny.Purge;
using SharpBunny.Statistics;

namespace SharpBunny;

public class BunnyGeneralApi
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://api.bunny.net";

    public CountriesService Countries { get; }
    public DnsZonesService DnsZones { get; }
    public PullZonesService PullZones { get; }
    public RegionsService Regions { get; }
    public PurgeService Purge { get; }
    public StatisticsService Statistics { get; }

    public BunnyGeneralApi(string apiKey) : this(new HttpClient(), apiKey)
    {
    }

    public BunnyGeneralApi(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("API key cannot be null or empty", nameof(apiKey));

        ConfigureHttpClient(apiKey);

        Countries = new CountriesService(_httpClient);
        DnsZones = new DnsZonesService(_httpClient);
        PullZones = new PullZonesService(_httpClient);
        Regions = new RegionsService(_httpClient);
        Purge = new PurgeService(_httpClient);
        Statistics = new StatisticsService(_httpClient);
    }

    private void ConfigureHttpClient(string apiKey)
    {
        _httpClient.BaseAddress = new Uri(BaseUrl);
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("AccessKey", apiKey);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("SharpBunny", "1.0.0"));
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
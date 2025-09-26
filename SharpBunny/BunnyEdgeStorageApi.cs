using System.Net.Http.Headers;
using SharpBunny.EdgeStorage;

namespace SharpBunny;

public class BunnyEdgeStorageApi
{
    private readonly HttpClient _httpClient;
    private const string DefaultBaseUrl = "https://storage.bunnycdn.com";

    public EdgeStorageService EdgeStorage { get; }

    public BunnyEdgeStorageApi(string storageZonePassword) : this(new HttpClient(), storageZonePassword)
    {
    }

    public BunnyEdgeStorageApi(HttpClient httpClient, string storageZonePassword)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        if (string.IsNullOrWhiteSpace(storageZonePassword))
            throw new ArgumentException("Storage zone password cannot be null or empty", nameof(storageZonePassword));

        ConfigureHttpClient(storageZonePassword);

        EdgeStorage = new EdgeStorageService(_httpClient);
    }

    private void ConfigureHttpClient(string storageZonePassword)
    {
        _httpClient.BaseAddress = new Uri(DefaultBaseUrl);
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("AccessKey", storageZonePassword);
        _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("SharpBunny", "1.0.0"));
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
using System.Net.Http.Headers;
using System.Text.Json;
using SharpBunny.Collections;
using SharpBunny.Videos;

namespace SharpBunny
{

    public class BunnyStreamApi
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://video.bunnycdn.com";

        public CollectionService Collections { get; }
        public VideoService Videos { get; }

        public BunnyStreamApi(string apiKey) : this(new HttpClient(), apiKey)
        {
        }

        public BunnyStreamApi(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("API key cannot be null or empty", nameof(apiKey));

            ConfigureHttpClient(apiKey);

            Collections = new CollectionService(_httpClient);
            Videos = new VideoService(_httpClient);
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
}
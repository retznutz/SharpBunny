using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using SharpBunny.Models;

namespace SharpBunny.Regions;

public class RegionsService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public RegionsService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <summary>
    /// Get a list of all regions
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of regions</returns>
    public async Task<List<Region>> GetRegionsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("/region", cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, content);
        }

        return JsonSerializer.Deserialize<List<Region>>(content, _jsonOptions) ?? new List<Region>();
    }

    [DoesNotReturn]
    private static void HandleErrorResponse(HttpResponseMessage response, string content)
    {
        try
        {
            var error = JsonSerializer.Deserialize<ApiError>(content);
            throw new BunnyApiException(error?.Detail ?? "Unknown error occurred", response.StatusCode, error);
        }
        catch (JsonException)
        {
            // If we can't parse the error response, throw a generic exception
            throw new BunnyApiException($"HTTP {(int)response.StatusCode}: {content}", response.StatusCode);
        }
    }
}
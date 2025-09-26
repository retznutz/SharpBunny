using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using SharpBunny.Models;

namespace SharpBunny.Purge;

public class PurgeService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public PurgeService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <summary>
    /// Purge cache for a pull zone
    /// </summary>
    /// <param name="pullZoneId">The pull zone ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task PurgePullZoneCacheAsync(
        int pullZoneId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync($"/pullzone/{pullZoneId}/purgeCache", null, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            HandleErrorResponse(response, content);
        }
    }

    /// <summary>
    /// Purge cache for specific URLs
    /// </summary>
    /// <param name="request">The purge request containing URLs to purge</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task PurgeUrlsAsync(
        PurgeUrlsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/purge", content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            HandleErrorResponse(response, responseContent);
        }
    }

    /// <summary>
    /// Get purge history
    /// </summary>
    /// <param name="page">The page number to return (default: 1)</param>
    /// <param name="perPage">The number of items per page (default: 1000, max: 1000)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of purge history entries</returns>
    public async Task<ApiResponse<PurgeHistoryEntry>> GetPurgeHistoryAsync(
        int page = 1,
        int perPage = 1000,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>
        {
            $"page={page}",
            $"perPage={Math.Min(perPage, 1000)}"
        };

        var queryString = string.Join("&", queryParams);
        var url = $"/purge?{queryString}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, content);
        }

        return JsonSerializer.Deserialize<ApiResponse<PurgeHistoryEntry>>(content, _jsonOptions) 
               ?? new ApiResponse<PurgeHistoryEntry>();
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
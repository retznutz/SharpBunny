using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using SharpBunny.Models;

namespace SharpBunny.PullZones;

public class PullZonesService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public PullZonesService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <summary>
    /// Get a list of all pull zones
    /// </summary>
    /// <param name="page">The page number to return (default: 1)</param>
    /// <param name="perPage">The number of items per page (default: 1000, max: 1000)</param>
    /// <param name="search">The search term to filter zones by name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of pull zones</returns>
    public async Task<ApiResponse<PullZone>> GetPullZonesAsync(
        int page = 1,
        int perPage = 1000,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>
        {
            $"page={page}",
            $"perPage={Math.Min(perPage, 1000)}"
        };

        if (!string.IsNullOrWhiteSpace(search))
            queryParams.Add($"search={Uri.EscapeDataString(search)}");

        var queryString = string.Join("&", queryParams);
        var url = $"/pullzone?{queryString}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, content);
        }

        return JsonSerializer.Deserialize<ApiResponse<PullZone>>(content, _jsonOptions) 
               ?? new ApiResponse<PullZone>();
    }

    /// <summary>
    /// Get a specific pull zone by ID
    /// </summary>
    /// <param name="pullZoneId">The pull zone ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The pull zone details</returns>
    public async Task<PullZone> GetPullZoneAsync(
        int pullZoneId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/pullzone/{pullZoneId}", cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, content);
        }

        return JsonSerializer.Deserialize<PullZone>(content, _jsonOptions) 
               ?? throw new InvalidOperationException("Failed to deserialize pull zone response");
    }

    /// <summary>
    /// Create a new pull zone
    /// </summary>
    /// <param name="request">The pull zone creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created pull zone</returns>
    public async Task<PullZone> CreatePullZoneAsync(
        CreatePullZoneRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/pullzone", content, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, responseContent);
        }

        return JsonSerializer.Deserialize<PullZone>(responseContent, _jsonOptions) 
               ?? throw new InvalidOperationException("Failed to deserialize pull zone response");
    }

    /// <summary>
    /// Update a pull zone
    /// </summary>
    /// <param name="pullZoneId">The pull zone ID</param>
    /// <param name="request">The pull zone update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated pull zone</returns>
    public async Task<PullZone> UpdatePullZoneAsync(
        int pullZoneId,
        UpdatePullZoneRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"/pullzone/{pullZoneId}", content, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, responseContent);
        }

        return JsonSerializer.Deserialize<PullZone>(responseContent, _jsonOptions) 
               ?? throw new InvalidOperationException("Failed to deserialize pull zone response");
    }

    /// <summary>
    /// Delete a pull zone
    /// </summary>
    /// <param name="pullZoneId">The pull zone ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task DeletePullZoneAsync(
        int pullZoneId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"/pullzone/{pullZoneId}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            HandleErrorResponse(response, content);
        }
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
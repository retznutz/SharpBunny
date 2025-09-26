using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using SharpBunny.Models;

namespace SharpBunny.DnsZones;

public class DnsZonesService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public DnsZonesService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <summary>
    /// Get a list of all DNS zones
    /// </summary>
    /// <param name="page">The page number to return (default: 1)</param>
    /// <param name="perPage">The number of items per page (default: 1000, max: 1000)</param>
    /// <param name="search">The search term to filter zones by domain</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of DNS zones</returns>
    public async Task<ApiResponse<DnsZone>> GetDnsZonesAsync(
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
        var url = $"/dnszone?{queryString}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, content);
        }

        return JsonSerializer.Deserialize<ApiResponse<DnsZone>>(content, _jsonOptions) 
               ?? new ApiResponse<DnsZone>();
    }

    /// <summary>
    /// Get a specific DNS zone by ID
    /// </summary>
    /// <param name="zoneId">The DNS zone ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The DNS zone details</returns>
    public async Task<DnsZone> GetDnsZoneAsync(
        int zoneId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/dnszone/{zoneId}", cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, content);
        }

        return JsonSerializer.Deserialize<DnsZone>(content, _jsonOptions) 
               ?? throw new InvalidOperationException("Failed to deserialize DNS zone response");
    }

    /// <summary>
    /// Create a new DNS zone
    /// </summary>
    /// <param name="domain">The domain name for the DNS zone</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created DNS zone</returns>
    public async Task<DnsZone> CreateDnsZoneAsync(
        string domain,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(domain))
            throw new ArgumentException("Domain cannot be null or empty", nameof(domain));

        var requestBody = new { domain };
        var json = JsonSerializer.Serialize(requestBody, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/dnszone", content, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, responseContent);
        }

        return JsonSerializer.Deserialize<DnsZone>(responseContent, _jsonOptions) 
               ?? throw new InvalidOperationException("Failed to deserialize DNS zone response");
    }

    /// <summary>
    /// Delete a DNS zone
    /// </summary>
    /// <param name="zoneId">The DNS zone ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task DeleteDnsZoneAsync(
        int zoneId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"/dnszone/{zoneId}", cancellationToken);

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
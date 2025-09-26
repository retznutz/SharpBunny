using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using SharpBunny.Models;

namespace SharpBunny.Statistics;

public class StatisticsService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public StatisticsService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <summary>
    /// Get statistics for a pull zone
    /// </summary>
    /// <param name="pullZoneId">The pull zone ID</param>
    /// <param name="dateFrom">Start date for statistics (optional)</param>
    /// <param name="dateTo">End date for statistics (optional)</param>
    /// <param name="hourly">Whether to get hourly breakdown (default: false)</param>
    /// <param name="loadErrors">Whether to include load errors in statistics (default: false)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Pull zone statistics</returns>
    public async Task<PullZoneStatistics> GetPullZoneStatisticsAsync(
        int pullZoneId,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        bool hourly = false,
        bool loadErrors = false,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>();

        if (dateFrom.HasValue)
            queryParams.Add($"dateFrom={dateFrom.Value:yyyy-MM-dd}");

        if (dateTo.HasValue)
            queryParams.Add($"dateTo={dateTo.Value:yyyy-MM-dd}");

        if (hourly)
            queryParams.Add("hourly=true");

        if (loadErrors)
            queryParams.Add("loadErrors=true");

        var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
        var url = $"/pullzone/{pullZoneId}/statistics{queryString}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, content);
        }

        return JsonSerializer.Deserialize<PullZoneStatistics>(content, _jsonOptions) 
               ?? new PullZoneStatistics();
    }

    /// <summary>
    /// Get billing statistics
    /// </summary>
    /// <param name="dateFrom">Start date for statistics (optional)</param>
    /// <param name="dateTo">End date for statistics (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Billing statistics</returns>
    public async Task<BillingStatistics> GetBillingStatisticsAsync(
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>();

        if (dateFrom.HasValue)
            queryParams.Add($"dateFrom={dateFrom.Value:yyyy-MM-dd}");

        if (dateTo.HasValue)
            queryParams.Add($"dateTo={dateTo.Value:yyyy-MM-dd}");

        var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
        var url = $"/statistics{queryString}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, content);
        }

        return JsonSerializer.Deserialize<BillingStatistics>(content, _jsonOptions) 
               ?? new BillingStatistics();
    }

    /// <summary>
    /// Get statistics by country for a pull zone
    /// </summary>
    /// <param name="pullZoneId">The pull zone ID</param>
    /// <param name="dateFrom">Start date for statistics (optional)</param>
    /// <param name="dateTo">End date for statistics (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Country statistics</returns>
    public async Task<List<CountryStatistics>> GetPullZoneCountryStatisticsAsync(
        int pullZoneId,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>();

        if (dateFrom.HasValue)
            queryParams.Add($"dateFrom={dateFrom.Value:yyyy-MM-dd}");

        if (dateTo.HasValue)
            queryParams.Add($"dateTo={dateTo.Value:yyyy-MM-dd}");

        var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
        var url = $"/pullzone/{pullZoneId}/statistics/countries{queryString}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, content);
        }

        return JsonSerializer.Deserialize<List<CountryStatistics>>(content, _jsonOptions) 
               ?? new List<CountryStatistics>();
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
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using SharpBunny.Models;

namespace SharpBunny.Collections;

public class CollectionService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public CollectionService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <summary>
    /// Get a paginated list of collections
    /// </summary>
    /// <param name="libraryId">The Video Library ID</param>
    /// <param name="page">The page number to return (default: 1)</param>
    /// <param name="itemsPerPage">The number of items per page (default: 100, max: 100)</param>
    /// <param name="search">The search term to filter collections by name</param>
    /// <param name="orderBy">Order collections by specified field (default: date)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of collections</returns>
    public async Task<ApiResponse<Collection>> GetCollectionsAsync(
        int libraryId,
        int page = 1,
        int itemsPerPage = 100,
        string? search = null,
        string? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>
        {
            $"page={page}",
            $"itemsPerPage={Math.Min(itemsPerPage, 100)}"
        };

        if (!string.IsNullOrWhiteSpace(search))
            queryParams.Add($"search={Uri.EscapeDataString(search)}");

        if (!string.IsNullOrWhiteSpace(orderBy))
            queryParams.Add($"orderBy={Uri.EscapeDataString(orderBy)}");

        var queryString = string.Join("&", queryParams);
        var url = $"/library/{libraryId}/collections?{queryString}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, content);
        }

        return JsonSerializer.Deserialize<ApiResponse<Collection>>(content, _jsonOptions) 
               ?? new ApiResponse<Collection>();
    }

    /// <summary>
    /// Get a specific collection by ID
    /// </summary>
    /// <param name="libraryId">The Video Library ID</param>
    /// <param name="collectionId">The collection ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The collection details</returns>
    public async Task<Collection> GetCollectionAsync(
        int libraryId,
        string collectionId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(collectionId))
            throw new ArgumentException("Collection ID cannot be null or empty", nameof(collectionId));

        var url = $"/library/{libraryId}/collections/{collectionId}";
        var response = await _httpClient.GetAsync(url, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, content);
        }

        return JsonSerializer.Deserialize<Collection>(content, _jsonOptions) 
               ?? throw new InvalidOperationException("Failed to deserialize collection response");
    }

    /// <summary>
    /// Create a new collection
    /// </summary>
    /// <param name="libraryId">The Video Library ID</param>
    /// <param name="name">The name of the collection</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created collection</returns>
    public async Task<Collection> CreateCollectionAsync(
        int libraryId,
        string name,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Collection name cannot be null or empty", nameof(name));

        var requestBody = new { name };
        var json = JsonSerializer.Serialize(requestBody, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var url = $"/library/{libraryId}/collections";
        var response = await _httpClient.PostAsync(url, content, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, responseContent);
        }

        return JsonSerializer.Deserialize<Collection>(responseContent, _jsonOptions) 
               ?? throw new InvalidOperationException("Failed to deserialize collection response");
    }

    /// <summary>
    /// Update a collection
    /// </summary>
    /// <param name="libraryId">The Video Library ID</param>
    /// <param name="collectionId">The collection ID</param>
    /// <param name="name">The new name of the collection</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated collection</returns>
    public async Task<Collection> UpdateCollectionAsync(
        int libraryId,
        string collectionId,
        string name,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(collectionId))
            throw new ArgumentException("Collection ID cannot be null or empty", nameof(collectionId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Collection name cannot be null or empty", nameof(name));

        var requestBody = new { name };
        var json = JsonSerializer.Serialize(requestBody, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var url = $"/library/{libraryId}/collections/{collectionId}";
        var response = await _httpClient.PostAsync(url, content, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, responseContent);
        }

        return JsonSerializer.Deserialize<Collection>(responseContent, _jsonOptions) 
               ?? throw new InvalidOperationException("Failed to deserialize collection response");
    }

    /// <summary>
    /// Delete a collection
    /// </summary>
    /// <param name="libraryId">The Video Library ID</param>
    /// <param name="collectionId">The collection ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task DeleteCollectionAsync(
        int libraryId,
        string collectionId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(collectionId))
            throw new ArgumentException("Collection ID cannot be null or empty", nameof(collectionId));

        var url = $"/library/{libraryId}/collections/{collectionId}";
        var response = await _httpClient.DeleteAsync(url, cancellationToken);

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
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using SharpBunny.Models;

namespace SharpBunny.Videos;

public class VideoService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public VideoService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <summary>
    /// Get a paginated list of videos
    /// </summary>
    /// <param name="libraryId">The Video Library ID</param>
    /// <param name="page">The page number to return (default: 1)</param>
    /// <param name="itemsPerPage">The number of items per page (default: 100, max: 100)</param>
    /// <param name="search">The search term to filter videos by title</param>
    /// <param name="collection">The collection ID to filter videos</param>
    /// <param name="orderBy">Order videos by specified field (default: date)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of videos</returns>
    public async Task<ApiResponse<Video>> GetVideosAsync(
        int libraryId,
        int page = 1,
        int itemsPerPage = 100,
        string? search = null,
        string? collection = null,
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

        if (!string.IsNullOrWhiteSpace(collection))
            queryParams.Add($"collection={Uri.EscapeDataString(collection)}");

        if (!string.IsNullOrWhiteSpace(orderBy))
            queryParams.Add($"orderBy={Uri.EscapeDataString(orderBy)}");

        var queryString = string.Join("&", queryParams);
        var url = $"/library/{libraryId}/videos?{queryString}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, content);
        }

        return JsonSerializer.Deserialize<ApiResponse<Video>>(content, _jsonOptions) 
               ?? new ApiResponse<Video>();
    }

    /// <summary>
    /// Get a specific video by ID
    /// </summary>
    /// <param name="libraryId">The Video Library ID</param>
    /// <param name="videoId">The video ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The video details</returns>
    public async Task<Video> GetVideoAsync(
        int libraryId,
        string videoId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(videoId))
            throw new ArgumentException("Video ID cannot be null or empty", nameof(videoId));

        var url = $"/library/{libraryId}/videos/{videoId}";
        var response = await _httpClient.GetAsync(url, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, content);
        }

        return JsonSerializer.Deserialize<Video>(content, _jsonOptions) 
               ?? throw new InvalidOperationException("Failed to deserialize video response");
    }

    /// <summary>
    /// Create a new video
    /// </summary>
    /// <param name="libraryId">The Video Library ID</param>
    /// <param name="title">The title of the video</param>
    /// <param name="collectionId">Optional collection ID to assign the video to</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created video</returns>
    public async Task<Video> CreateVideoAsync(
        int libraryId,
        string title,
        string? collectionId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Video title cannot be null or empty", nameof(title));

        var requestBody = new
        {
            title,
            collectionId
        };

        var json = JsonSerializer.Serialize(requestBody, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var url = $"/library/{libraryId}/videos";
        var response = await _httpClient.PostAsync(url, content, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, responseContent);
        }

        return JsonSerializer.Deserialize<Video>(responseContent, _jsonOptions) 
               ?? throw new InvalidOperationException("Failed to deserialize video response");
    }

    /// <summary>
    /// Update a video
    /// </summary>
    /// <param name="libraryId">The Video Library ID</param>
    /// <param name="videoId">The video ID</param>
    /// <param name="title">The new title of the video</param>
    /// <param name="collectionId">Optional collection ID to assign the video to</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated video</returns>
    public async Task<Video> UpdateVideoAsync(
        int libraryId,
        string videoId,
        string? title = null,
        string? collectionId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(videoId))
            throw new ArgumentException("Video ID cannot be null or empty", nameof(videoId));

        var requestBody = new Dictionary<string, object?>();
        
        if (!string.IsNullOrWhiteSpace(title))
            requestBody["title"] = title;
            
        if (collectionId != null)
            requestBody["collectionId"] = collectionId;

        if (requestBody.Count == 0)
            throw new ArgumentException("At least one field must be provided for update");

        var json = JsonSerializer.Serialize(requestBody, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var url = $"/library/{libraryId}/videos/{videoId}";
        var response = await _httpClient.PostAsync(url, content, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, responseContent);
        }

        return JsonSerializer.Deserialize<Video>(responseContent, _jsonOptions) 
               ?? throw new InvalidOperationException("Failed to deserialize video response");
    }

    /// <summary>
    /// Delete a video
    /// </summary>
    /// <param name="libraryId">The Video Library ID</param>
    /// <param name="videoId">The video ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task DeleteVideoAsync(
        int libraryId,
        string videoId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(videoId))
            throw new ArgumentException("Video ID cannot be null or empty", nameof(videoId));

        var url = $"/library/{libraryId}/videos/{videoId}";
        var response = await _httpClient.DeleteAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            HandleErrorResponse(response, content);
        }
    }

    /// <summary>
    /// Upload a video file
    /// </summary>
    /// <param name="libraryId">The Video Library ID</param>
    /// <param name="videoId">The video ID</param>
    /// <param name="fileStream">The video file stream</param>
    /// <param name="fileName">The file name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The upload result</returns>
    public async Task<bool> UploadVideoAsync(
        int libraryId,
        string videoId,
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(videoId))
            throw new ArgumentException("Video ID cannot be null or empty", nameof(videoId));

        if (fileStream == null)
            throw new ArgumentNullException(nameof(fileStream));

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be null or empty", nameof(fileName));

        using var content = new MultipartFormDataContent();
        using var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
        content.Add(streamContent, "file", fileName);

        var url = $"/library/{libraryId}/videos/{videoId}";
        var response = await _httpClient.PutAsync(url, content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            HandleErrorResponse(response, responseContent);
        }

        return response.IsSuccessStatusCode;
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
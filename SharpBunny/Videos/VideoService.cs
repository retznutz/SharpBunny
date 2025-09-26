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

    /// <summary>
    /// Get video heatmap data
    /// </summary>
    /// <param name="libraryId">The Video Library ID</param>
    /// <param name="videoId">The video ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The video heatmap data</returns>
    public async Task<VideoHeatmapData> GetVideoHeatmapAsync(
        int libraryId,
        string videoId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(videoId))
            throw new ArgumentException("Video ID cannot be null or empty", nameof(videoId));

        var url = $"/library/{libraryId}/videos/{videoId}/heatmap";
        var response = await _httpClient.GetAsync(url, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, content);
        }

        return JsonSerializer.Deserialize<VideoHeatmapData>(content, _jsonOptions) 
               ?? throw new InvalidOperationException("Failed to deserialize video heatmap response");
    }

    /// <summary>
    /// Get video play data statistics
    /// </summary>
    /// <param name="libraryId">The Video Library ID</param>
    /// <param name="videoId">The video ID</param>
    /// <param name="dateFrom">Start date for statistics (optional)</param>
    /// <param name="dateTo">End date for statistics (optional)</param>
    /// <param name="hourly">Whether to get hourly data (default: false)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of video play data</returns>
    public async Task<List<VideoPlayData>> GetVideoPlayDataAsync(
        int libraryId,
        string videoId,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        bool hourly = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(videoId))
            throw new ArgumentException("Video ID cannot be null or empty", nameof(videoId));

        var queryParams = new List<string>();
        
        if (dateFrom.HasValue)
            queryParams.Add($"dateFrom={dateFrom.Value:yyyy-MM-dd}");
            
        if (dateTo.HasValue)
            queryParams.Add($"dateTo={dateTo.Value:yyyy-MM-dd}");
            
        if (hourly)
            queryParams.Add("hourly=true");

        var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
        var url = $"/library/{libraryId}/videos/{videoId}/play{queryString}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, content);
        }

        return JsonSerializer.Deserialize<List<VideoPlayData>>(content, _jsonOptions) 
               ?? new List<VideoPlayData>();
    }

    /// <summary>
    /// Get video statistics
    /// </summary>
    /// <param name="libraryId">The Video Library ID</param>
    /// <param name="videoId">The video ID</param>
    /// <param name="dateFrom">Start date for statistics (optional)</param>
    /// <param name="dateTo">End date for statistics (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The video statistics</returns>
    public async Task<VideoStatistics> GetVideoStatisticsAsync(
        int libraryId,
        string videoId,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(videoId))
            throw new ArgumentException("Video ID cannot be null or empty", nameof(videoId));

        var queryParams = new List<string>();
        
        if (dateFrom.HasValue)
            queryParams.Add($"dateFrom={dateFrom.Value:yyyy-MM-dd}");
            
        if (dateTo.HasValue)
            queryParams.Add($"dateTo={dateTo.Value:yyyy-MM-dd}");

        var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
        var url = $"/library/{libraryId}/videos/{videoId}/statistics{queryString}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, content);
        }

        return JsonSerializer.Deserialize<VideoStatistics>(content, _jsonOptions) 
               ?? throw new InvalidOperationException("Failed to deserialize video statistics response");
    }

    /// <summary>
    /// Get detailed video heatmap data
    /// </summary>
    /// <param name="libraryId">The Video Library ID</param>
    /// <param name="videoId">The video ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The detailed video heatmap data</returns>
    public async Task<List<VideoHeatmap>> GetVideoHeatmapDataAsync(
        int libraryId,
        string videoId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(videoId))
            throw new ArgumentException("Video ID cannot be null or empty", nameof(videoId));

        var url = $"/library/{libraryId}/videos/{videoId}/heatmapdata";
        var response = await _httpClient.GetAsync(url, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, content);
        }

        return JsonSerializer.Deserialize<List<VideoHeatmap>>(content, _jsonOptions) 
               ?? new List<VideoHeatmap>();
    }

    /// <summary>
    /// Re-encode a video
    /// </summary>
    /// <param name="libraryId">The Video Library ID</param>
    /// <param name="videoId">The video ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The re-encode result</returns>
    public async Task<VideoReencodeResult> ReencodeVideoAsync(
        int libraryId,
        string videoId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(videoId))
            throw new ArgumentException("Video ID cannot be null or empty", nameof(videoId));

        var url = $"/library/{libraryId}/videos/{videoId}/reencode";
        var response = await _httpClient.PostAsync(url, null, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, content);
        }

        return JsonSerializer.Deserialize<VideoReencodeResult>(content, _jsonOptions) 
               ?? throw new InvalidOperationException("Failed to deserialize video reencode response");
    }

    /// <summary>
    /// Re-encode a video using a specific codec
    /// </summary>
    /// <param name="libraryId">The Video Library ID</param>
    /// <param name="videoId">The video ID</param>
    /// <param name="codec">The codec to use for re-encoding</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The re-encode result</returns>
    public async Task<VideoReencodeResult> ReencodeUsingCodecAsync(
        int libraryId,
        string videoId,
        string codec,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(videoId))
            throw new ArgumentException("Video ID cannot be null or empty", nameof(videoId));

        if (string.IsNullOrWhiteSpace(codec))
            throw new ArgumentException("Codec cannot be null or empty", nameof(codec));

        var requestBody = new { codec };
        var json = JsonSerializer.Serialize(requestBody, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var url = $"/library/{libraryId}/videos/{videoId}/reencode";
        var response = await _httpClient.PostAsync(url, content, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, responseContent);
        }

        return JsonSerializer.Deserialize<VideoReencodeResult>(responseContent, _jsonOptions) 
               ?? throw new InvalidOperationException("Failed to deserialize video reencode response");
    }

    /// <summary>
    /// Repackage a video
    /// </summary>
    /// <param name="libraryId">The Video Library ID</param>
    /// <param name="videoId">The video ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The repackage result</returns>
    public async Task<VideoReencodeResult> RepackageVideoAsync(
        int libraryId,
        string videoId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(videoId))
            throw new ArgumentException("Video ID cannot be null or empty", nameof(videoId));

        var url = $"/library/{libraryId}/videos/{videoId}/repackage";
        var response = await _httpClient.PostAsync(url, null, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, content);
        }

        return JsonSerializer.Deserialize<VideoReencodeResult>(content, _jsonOptions) 
               ?? throw new InvalidOperationException("Failed to deserialize video repackage response");
    }

    /// <summary>
    /// Set video thumbnail
    /// </summary>
    /// <param name="libraryId">The Video Library ID</param>
    /// <param name="videoId">The video ID</param>
    /// <param name="thumbnailUrl">The URL of the thumbnail image</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated video</returns>
    public async Task<Video> SetThumbnailAsync(
        int libraryId,
        string videoId,
        string thumbnailUrl,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(videoId))
            throw new ArgumentException("Video ID cannot be null or empty", nameof(videoId));

        if (string.IsNullOrWhiteSpace(thumbnailUrl))
            throw new ArgumentException("Thumbnail URL cannot be null or empty", nameof(thumbnailUrl));

        var requestBody = new { thumbnailUrl };
        var json = JsonSerializer.Serialize(requestBody, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var url = $"/library/{libraryId}/videos/{videoId}/thumbnail";
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
    /// Add caption to video
    /// </summary>
    /// <param name="libraryId">The Video Library ID</param>
    /// <param name="videoId">The video ID</param>
    /// <param name="srclang">The language code for the caption</param>
    /// <param name="label">The label for the caption</param>
    /// <param name="captionFile">The caption file stream</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated video</returns>
    public async Task<Video> AddCaptionAsync(
        int libraryId,
        string videoId,
        string srclang,
        string label,
        Stream captionFile,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(videoId))
            throw new ArgumentException("Video ID cannot be null or empty", nameof(videoId));

        if (string.IsNullOrWhiteSpace(srclang))
            throw new ArgumentException("Source language cannot be null or empty", nameof(srclang));

        if (string.IsNullOrWhiteSpace(label))
            throw new ArgumentException("Label cannot be null or empty", nameof(label));

        if (captionFile == null)
            throw new ArgumentNullException(nameof(captionFile));

        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(srclang), "srclang");
        content.Add(new StringContent(label), "label");
        
        using var streamContent = new StreamContent(captionFile);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/vtt");
        content.Add(streamContent, "captionsFile", $"{srclang}.vtt");

        var url = $"/library/{libraryId}/videos/{videoId}/captions/{srclang}";
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
    /// Delete caption from video
    /// </summary>
    /// <param name="libraryId">The Video Library ID</param>
    /// <param name="videoId">The video ID</param>
    /// <param name="srclang">The language code of the caption to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task DeleteCaptionAsync(
        int libraryId,
        string videoId,
        string srclang,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(videoId))
            throw new ArgumentException("Video ID cannot be null or empty", nameof(videoId));

        if (string.IsNullOrWhiteSpace(srclang))
            throw new ArgumentException("Source language cannot be null or empty", nameof(srclang));

        var url = $"/library/{libraryId}/videos/{videoId}/captions/{srclang}";
        var response = await _httpClient.DeleteAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            HandleErrorResponse(response, content);
        }
    }

    /// <summary>
    /// Transcribe video to generate captions
    /// </summary>
    /// <param name="libraryId">The Video Library ID</param>
    /// <param name="videoId">The video ID</param>
    /// <param name="language">The language to transcribe to (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The transcription result</returns>
    public async Task<VideoReencodeResult> TranscribeVideoAsync(
        int libraryId,
        string videoId,
        string? language = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(videoId))
            throw new ArgumentException("Video ID cannot be null or empty", nameof(videoId));

        var requestBody = language != null ? new { language } : null;
        StringContent? content = null;

        if (requestBody != null)
        {
            var json = JsonSerializer.Serialize(requestBody, _jsonOptions);
            content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        var url = $"/library/{libraryId}/videos/{videoId}/transcribe";
        var response = await _httpClient.PostAsync(url, content, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, responseContent);
        }

        return JsonSerializer.Deserialize<VideoReencodeResult>(responseContent, _jsonOptions) 
               ?? throw new InvalidOperationException("Failed to deserialize transcribe response");
    }

    /// <summary>
    /// Get available video resolutions
    /// </summary>
    /// <param name="libraryId">The Video Library ID</param>
    /// <param name="videoId">The video ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of available video resolutions</returns>
    public async Task<List<VideoResolution>> GetVideoResolutionsAsync(
        int libraryId,
        string videoId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(videoId))
            throw new ArgumentException("Video ID cannot be null or empty", nameof(videoId));

        var url = $"/library/{libraryId}/videos/{videoId}/resolutions";
        var response = await _httpClient.GetAsync(url, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, content);
        }

        return JsonSerializer.Deserialize<List<VideoResolution>>(content, _jsonOptions) 
               ?? new List<VideoResolution>();
    }

    /// <summary>
    /// Delete video resolutions
    /// </summary>
    /// <param name="libraryId">The Video Library ID</param>
    /// <param name="videoId">The video ID</param>
    /// <param name="resolutions">List of resolution identifiers to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task DeleteResolutionsAsync(
        int libraryId,
        string videoId,
        List<string> resolutions,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(videoId))
            throw new ArgumentException("Video ID cannot be null or empty", nameof(videoId));

        if (resolutions == null || resolutions.Count == 0)
            throw new ArgumentException("Resolutions list cannot be null or empty", nameof(resolutions));

        var requestBody = new { resolutions };
        var json = JsonSerializer.Serialize(requestBody, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var url = $"/library/{libraryId}/videos/{videoId}/resolutions";
        var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, url) { Content = content }, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            HandleErrorResponse(response, responseContent);
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
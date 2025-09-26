using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using SharpBunny.Models;

namespace SharpBunny.EdgeStorage;

public class EdgeStorageService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public EdgeStorageService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <summary>
    /// List files and directories in a storage zone path
    /// </summary>
    /// <param name="storageZoneName">The name of your storage zone</param>
    /// <param name="path">The directory path to list (optional, defaults to root)</param>
    /// <param name="storageZonePassword">The storage zone password</param>
    /// <param name="storageZoneEndpoint">The storage API endpoint (optional, defaults to storage.bunnycdn.com)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of files and directories</returns>
    public async Task<List<StorageFile>> ListFilesAsync(
        string storageZoneName,
        string? path = null,
        string? storageZonePassword = null,
        string? storageZoneEndpoint = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storageZoneName))
            throw new ArgumentException("Storage zone name cannot be null or empty", nameof(storageZoneName));

        var endpoint = storageZoneEndpoint ?? "storage.bunnycdn.com";
        var normalizedPath = NormalizePath(path);
        var fullPath = $"{storageZoneName}/{normalizedPath}";
        var url = $"https://{endpoint}/{fullPath}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Accept", "application/json");
        
        if (!string.IsNullOrWhiteSpace(storageZonePassword))
        {
            request.Headers.Add("AccessKey", storageZonePassword);
        }

        var response = await _httpClient.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, content);
        }

        return JsonSerializer.Deserialize<List<StorageFile>>(content, _jsonOptions) 
               ?? new List<StorageFile>();
    }

    /// <summary>
    /// Upload a file to the storage zone
    /// </summary>
    /// <param name="storageZoneName">The name of your storage zone</param>
    /// <param name="fileName">The name that the file will be uploaded as</param>
    /// <param name="fileContent">The file content as a byte array</param>
    /// <param name="path">The directory path where the file will be stored (optional)</param>
    /// <param name="storageZonePassword">The storage zone password</param>
    /// <param name="storageZoneEndpoint">The storage API endpoint (optional, defaults to storage.bunnycdn.com)</param>
    /// <param name="checksum">The hex-encoded SHA256 checksum of the uploaded content (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task UploadFileAsync(
        string storageZoneName,
        string fileName,
        byte[] fileContent,
        string? path = null,
        string? storageZonePassword = null,
        string? storageZoneEndpoint = null,
        string? checksum = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storageZoneName))
            throw new ArgumentException("Storage zone name cannot be null or empty", nameof(storageZoneName));

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be null or empty", nameof(fileName));

        if (fileContent == null)
            throw new ArgumentNullException(nameof(fileContent));

        var endpoint = storageZoneEndpoint ?? "storage.bunnycdn.com";
        var normalizedPath = NormalizePath(path);
        var fullPath = $"{storageZoneName}/{normalizedPath}/{fileName}";
        var url = $"https://{endpoint}/{fullPath}";

        var request = new HttpRequestMessage(HttpMethod.Put, url);
        request.Content = new ByteArrayContent(fileContent);
        request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
        
        if (!string.IsNullOrWhiteSpace(storageZonePassword))
        {
            request.Headers.Add("AccessKey", storageZonePassword);
        }

        if (!string.IsNullOrWhiteSpace(checksum))
        {
            request.Headers.Add("Checksum", checksum);
        }

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            HandleErrorResponse(response, content);
        }
    }

    /// <summary>
    /// Download a file from the storage zone
    /// </summary>
    /// <param name="storageZoneName">The name of your storage zone</param>
    /// <param name="fileName">The name of the file to download</param>
    /// <param name="path">The directory path where the file is located (optional)</param>
    /// <param name="storageZonePassword">The storage zone password</param>
    /// <param name="storageZoneEndpoint">The storage API endpoint (optional, defaults to storage.bunnycdn.com)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The file content as a byte array</returns>
    public async Task<byte[]> DownloadFileAsync(
        string storageZoneName,
        string fileName,
        string? path = null,
        string? storageZonePassword = null,
        string? storageZoneEndpoint = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storageZoneName))
            throw new ArgumentException("Storage zone name cannot be null or empty", nameof(storageZoneName));

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be null or empty", nameof(fileName));

        var endpoint = storageZoneEndpoint ?? "storage.bunnycdn.com";
        var normalizedPath = NormalizePath(path);
        var fullPath = $"{storageZoneName}/{normalizedPath}/{fileName}";
        var url = $"https://{endpoint}/{fullPath}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Accept", "*/*");
        
        if (!string.IsNullOrWhiteSpace(storageZonePassword))
        {
            request.Headers.Add("AccessKey", storageZonePassword);
        }

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            HandleErrorResponse(response, content);
        }

        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }

    /// <summary>
    /// Delete a file from the storage zone
    /// </summary>
    /// <param name="storageZoneName">The name of your storage zone</param>
    /// <param name="fileName">The name of the file to delete</param>
    /// <param name="path">The directory path where the file is located (optional)</param>
    /// <param name="storageZonePassword">The storage zone password</param>
    /// <param name="storageZoneEndpoint">The storage API endpoint (optional, defaults to storage.bunnycdn.com)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task DeleteFileAsync(
        string storageZoneName,
        string fileName,
        string? path = null,
        string? storageZonePassword = null,
        string? storageZoneEndpoint = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storageZoneName))
            throw new ArgumentException("Storage zone name cannot be null or empty", nameof(storageZoneName));

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be null or empty", nameof(fileName));

        var endpoint = storageZoneEndpoint ?? "storage.bunnycdn.com";
        var normalizedPath = NormalizePath(path);
        var fullPath = $"{storageZoneName}/{normalizedPath}/{fileName}";
        var url = $"https://{endpoint}/{fullPath}";

        var request = new HttpRequestMessage(HttpMethod.Delete, url);
        
        if (!string.IsNullOrWhiteSpace(storageZonePassword))
        {
            request.Headers.Add("AccessKey", storageZonePassword);
        }

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            HandleErrorResponse(response, content);
        }
    }

    private static string NormalizePath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return string.Empty;

        // Remove leading and trailing slashes and normalize
        return path.Trim('/').Replace('\\', '/');
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
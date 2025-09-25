using System.Net;
using System.Text;
using System.Text.Json;
using Moq;
using Moq.Protected;
using SharpBunny.Collections;
using SharpBunny.Models;
using Xunit;

namespace SharpBunny.Tests.Collections;

public class CollectionServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly HttpClient _httpClient;
    private readonly CollectionService _collectionService;

    public CollectionServiceTests()
    {
        _mockHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHandler.Object)
        {
            BaseAddress = new Uri("https://video.bunnycdn.com")
        };
        _collectionService = new CollectionService(_httpClient);
    }

    [Fact]
    public void Constructor_WithValidHttpClient_CreatesInstance()
    {
        // Arrange
        using var httpClient = new HttpClient();

        // Act
        var service = new CollectionService(httpClient);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CollectionService(null!));
    }

    [Fact]
    public async Task GetCollectionsAsync_WithValidParameters_ReturnsCollections()
    {
        // Arrange
        var libraryId = 123;
        var mockResponse = new ApiResponse<Collection>
        {
            TotalItems = 1,
            CurrentPage = 1,
            ItemsPerPage = 100,
            Items = new List<Collection>
            {
                new()
                {
                    VideoLibraryId = libraryId,
                    Guid = "test-guid",
                    Name = "Test Collection",
                    VideoCount = 5,
                    TotalSize = 1024
                }
            }
        };

        var json = JsonSerializer.Serialize(mockResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _collectionService.GetCollectionsAsync(libraryId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("Test Collection", result.Items[0].Name);
        Assert.Equal(libraryId, result.Items[0].VideoLibraryId);
    }

    [Fact]
    public async Task GetCollectionAsync_WithValidId_ReturnsCollection()
    {
        // Arrange
        var libraryId = 123;
        var collectionId = "test-collection-id";
        var mockResponse = new Collection
        {
            VideoLibraryId = libraryId,
            Guid = collectionId,
            Name = "Test Collection",
            VideoCount = 5,
            TotalSize = 1024
        };

        var json = JsonSerializer.Serialize(mockResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _collectionService.GetCollectionAsync(libraryId, collectionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Collection", result.Name);
        Assert.Equal(libraryId, result.VideoLibraryId);
        Assert.Equal(collectionId, result.Guid);
    }

    [Fact]
    public async Task GetCollectionAsync_WithNullCollectionId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _collectionService.GetCollectionAsync(123, null!));
    }

    [Fact]
    public async Task CreateCollectionAsync_WithValidName_ReturnsCreatedCollection()
    {
        // Arrange
        var libraryId = 123;
        var collectionName = "New Collection";
        var mockResponse = new Collection
        {
            VideoLibraryId = libraryId,
            Guid = "new-collection-guid",
            Name = collectionName,
            VideoCount = 0,
            TotalSize = 0
        };

        var json = JsonSerializer.Serialize(mockResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Created,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _collectionService.CreateCollectionAsync(libraryId, collectionName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(collectionName, result.Name);
        Assert.Equal(libraryId, result.VideoLibraryId);
    }

    [Fact]
    public async Task CreateCollectionAsync_WithNullName_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _collectionService.CreateCollectionAsync(123, null!));
    }

    [Fact]
    public async Task GetCollectionsAsync_WithHttpError_ThrowsBunnyApiException()
    {
        // Arrange
        var libraryId = 123;
        var errorResponse = new ApiError
        {
            Title = "Not Found",
            Status = 404,
            Detail = "Library not found"
        };

        var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BunnyApiException>(() => 
            _collectionService.GetCollectionsAsync(libraryId));
        
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Equal("Library not found", exception.Message);
    }
}
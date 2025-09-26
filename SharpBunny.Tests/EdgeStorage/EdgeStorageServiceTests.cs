using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Moq;
using Moq.Protected;
using SharpBunny.EdgeStorage;
using SharpBunny.Models;

namespace SharpBunny.Tests.EdgeStorage;

public class EdgeStorageServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly EdgeStorageService _edgeStorageService;

    public EdgeStorageServiceTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _edgeStorageService = new EdgeStorageService(_httpClient);
    }

    [Fact]
    public async Task ListFilesAsync_WithValidParameters_ReturnsStorageFiles()
    {
        // Arrange
        var storageZoneName = "test-zone";
        var path = "subpath";
        var storageZonePassword = "test-password";
        
        var storageFiles = new List<StorageFile>
        {
            new()
            {
                ObjectName = "test-file.txt",
                IsDirectory = false,
                Length = 1024,
                ContentType = "text/plain"
            },
            new()
            {
                ObjectName = "test-folder",
                IsDirectory = true,
                Length = 0,
                ContentType = ""
            }
        };

        var jsonResponse = JsonSerializer.Serialize(storageFiles);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _edgeStorageService.ListFilesAsync(storageZoneName, path, storageZonePassword);

        // Assert
        result.Should().HaveCount(2);
        result[0].ObjectName.Should().Be("test-file.txt");
        result[0].IsDirectory.Should().BeFalse();
        result[1].ObjectName.Should().Be("test-folder");
        result[1].IsDirectory.Should().BeTrue();
    }

    [Fact]
    public async Task ListFilesAsync_WithNullStorageZoneName_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _edgeStorageService.ListFilesAsync(null!));
    }

    [Fact]
    public async Task UploadFileAsync_WithValidParameters_Completes()
    {
        // Arrange
        var storageZoneName = "test-zone";
        var fileName = "test-file.txt";
        var fileContent = Encoding.UTF8.GetBytes("Hello, World!");
        var path = "uploads";
        var storageZonePassword = "test-password";

        var httpResponse = new HttpResponseMessage(HttpStatusCode.Created);

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        await _edgeStorageService.UploadFileAsync(storageZoneName, fileName, fileContent, path, storageZonePassword);

        // Assert
        // No exception should be thrown
    }

    [Fact]
    public async Task UploadFileAsync_WithNullFileName_ThrowsArgumentException()
    {
        // Arrange
        var storageZoneName = "test-zone";
        var fileContent = Encoding.UTF8.GetBytes("Hello, World!");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _edgeStorageService.UploadFileAsync(storageZoneName, null!, fileContent));
    }

    [Fact]
    public async Task DownloadFileAsync_WithValidParameters_ReturnsFileContent()
    {
        // Arrange
        var storageZoneName = "test-zone";
        var fileName = "test-file.txt";
        var expectedContent = Encoding.UTF8.GetBytes("Hello, World!");
        var storageZonePassword = "test-password";

        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(expectedContent)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _edgeStorageService.DownloadFileAsync(storageZoneName, fileName, storageZonePassword: storageZonePassword);

        // Assert
        result.Should().BeEquivalentTo(expectedContent);
    }

    [Fact]
    public async Task DeleteFileAsync_WithValidParameters_Completes()
    {
        // Arrange
        var storageZoneName = "test-zone";
        var fileName = "test-file.txt";
        var storageZonePassword = "test-password";

        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        await _edgeStorageService.DeleteFileAsync(storageZoneName, fileName, storageZonePassword: storageZonePassword);

        // Assert
        // No exception should be thrown
    }

    [Fact]
    public async Task DeleteFileAsync_WithNullFileName_ThrowsArgumentException()
    {
        // Arrange
        var storageZoneName = "test-zone";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _edgeStorageService.DeleteFileAsync(storageZoneName, null!));
    }
}
using System.Net;
using System.Text;
using System.Text.Json;
using Moq;
using Moq.Protected;
using SharpBunny.Models;
using SharpBunny.PullZones;
using Xunit;

namespace SharpBunny.Tests.PullZones;

public class PullZonesServiceTests
{
    [Fact]
    public void Constructor_WithValidHttpClient_CreatesInstance()
    {
        // Arrange
        var httpClient = new HttpClient();

        // Act
        var service = new PullZonesService(httpClient);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PullZonesService(null!));
    }

    [Fact]
    public async Task GetPullZonesAsync_WithValidResponse_ReturnsPullZones()
    {
        // Arrange
        var pullZones = new List<PullZone>
        {
            new() { Id = 1, Name = "Test Zone 1", OriginUrl = "https://example.com", Enabled = true },
            new() { Id = 2, Name = "Test Zone 2", OriginUrl = "https://example2.com", Enabled = false }
        };

        var response = new ApiResponse<PullZone>
        {
            Items = pullZones,
            TotalItems = 2,
            CurrentPage = 1,
            ItemsPerPage = 1000
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("https://api.bunny.net")
        };
        var service = new PullZonesService(httpClient);

        // Act
        var result = await service.GetPullZonesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal("Test Zone 1", result.Items[0].Name);
        Assert.Equal("https://example.com", result.Items[0].OriginUrl);
        Assert.True(result.Items[0].Enabled);
        Assert.Equal("Test Zone 2", result.Items[1].Name);
        Assert.False(result.Items[1].Enabled);
    }

    [Fact]
    public async Task CreatePullZoneAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        var httpClient = new HttpClient();
        var service = new PullZonesService(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreatePullZoneAsync(null!));
    }
}
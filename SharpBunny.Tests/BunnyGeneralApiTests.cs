using Xunit;

namespace SharpBunny.Tests;

public class BunnyGeneralApiTests
{
    [Fact]
    public void Constructor_WithValidApiKey_CreatesInstance()
    {
        // Arrange
        var apiKey = "test-api-key";

        // Act
        var api = new BunnyGeneralApi(apiKey);

        // Assert
        Assert.NotNull(api);
        Assert.NotNull(api.Countries);
        Assert.NotNull(api.DnsZones);
        Assert.NotNull(api.PullZones);
        Assert.NotNull(api.Regions);
        Assert.NotNull(api.Purge);
        Assert.NotNull(api.Statistics);
    }

    [Fact]
    public void Constructor_WithNullApiKey_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new BunnyGeneralApi(null!));
    }

    [Fact]
    public void Constructor_WithEmptyApiKey_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new BunnyGeneralApi(""));
    }

    [Fact]
    public void Constructor_WithWhiteSpaceApiKey_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new BunnyGeneralApi("   "));
    }

    [Fact]
    public void Constructor_WithHttpClientAndValidApiKey_CreatesInstance()
    {
        // Arrange
        var httpClient = new HttpClient();
        var apiKey = "test-api-key";

        // Act
        var api = new BunnyGeneralApi(httpClient, apiKey);

        // Assert
        Assert.NotNull(api);
        Assert.NotNull(api.Countries);
        Assert.NotNull(api.DnsZones);
        Assert.NotNull(api.PullZones);
        Assert.NotNull(api.Regions);
        Assert.NotNull(api.Purge);
        Assert.NotNull(api.Statistics);
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BunnyGeneralApi(null!, "test-api-key"));
    }
}
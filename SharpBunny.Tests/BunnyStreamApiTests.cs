using System.Net;
using Xunit;

namespace SharpBunny.Tests;

public class BunnyStreamApiTests
{
    [Fact]
    public void Constructor_WithValidApiKey_CreatesInstance()
    {
        // Arrange
        var apiKey = "test-api-key";

        // Act
        var api = new BunnyStreamApi(apiKey);

        // Assert
        Assert.NotNull(api);
        Assert.NotNull(api.Collections);
        Assert.NotNull(api.Videos);
    }

    [Fact]
    public void Constructor_WithNullApiKey_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new BunnyStreamApi(null!));
    }

    [Fact]
    public void Constructor_WithEmptyApiKey_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new BunnyStreamApi(string.Empty));
    }

    [Fact]
    public void Constructor_WithWhitespaceApiKey_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new BunnyStreamApi("   "));
    }

    [Fact]
    public void Constructor_WithHttpClient_CreatesInstance()
    {
        // Arrange
        using var httpClient = new HttpClient();
        var apiKey = "test-api-key";

        // Act
        var api = new BunnyStreamApi(httpClient, apiKey);

        // Assert
        Assert.NotNull(api);
        Assert.NotNull(api.Collections);
        Assert.NotNull(api.Videos);
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BunnyStreamApi(null!, "test-api-key"));
    }
}
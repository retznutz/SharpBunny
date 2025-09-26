using FluentAssertions;

namespace SharpBunny.Tests;

public class BunnyEdgeStorageApiTests
{
    [Fact]
    public void Constructor_WithValidApiKey_CreatesInstance()
    {
        // Arrange
        var storageZonePassword = "test-password";

        // Act
        var api = new BunnyEdgeStorageApi(storageZonePassword);

        // Assert
        api.Should().NotBeNull();
        api.EdgeStorage.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullApiKey_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new BunnyEdgeStorageApi(null!));
    }

    [Fact]
    public void Constructor_WithEmptyApiKey_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new BunnyEdgeStorageApi(string.Empty));
    }

    [Fact]
    public void Constructor_WithHttpClient_CreatesInstance()
    {
        // Arrange
        var httpClient = new HttpClient();
        var storageZonePassword = "test-password";

        // Act
        var api = new BunnyEdgeStorageApi(httpClient, storageZonePassword);

        // Assert
        api.Should().NotBeNull();
        api.EdgeStorage.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Arrange
        var storageZonePassword = "test-password";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BunnyEdgeStorageApi(null!, storageZonePassword));
    }
}
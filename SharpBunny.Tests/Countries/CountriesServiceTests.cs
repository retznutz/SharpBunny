using System.Net;
using System.Text;
using System.Text.Json;
using Moq;
using Moq.Protected;
using SharpBunny.Countries;
using SharpBunny.Models;
using Xunit;

namespace SharpBunny.Tests.Countries;

public class CountriesServiceTests
{
    [Fact]
    public void Constructor_WithValidHttpClient_CreatesInstance()
    {
        // Arrange
        var httpClient = new HttpClient();

        // Act
        var service = new CountriesService(httpClient);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CountriesService(null!));
    }

    [Fact]
    public async Task GetCountriesAsync_WithValidResponse_ReturnsCountries()
    {
        // Arrange
        var countries = new List<Country>
        {
            new() { Id = 1, Name = "United States", Code = "US", ContinentCode = "NA", IsEU = false, TaxRate = 0 },
            new() { Id = 2, Name = "Germany", Code = "DE", ContinentCode = "EU", IsEU = true, TaxRate = 19 }
        };

        var json = JsonSerializer.Serialize(countries, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
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
        var service = new CountriesService(httpClient);

        // Act
        var result = await service.GetCountriesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("United States", result[0].Name);
        Assert.Equal("US", result[0].Code);
        Assert.Equal("Germany", result[1].Name);
        Assert.Equal("DE", result[1].Code);
        Assert.True(result[1].IsEU);
    }
}
using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Moq;
using Moq.Protected;
using SharpBunny.Models;
using SharpBunny.Videos;
using Xunit;

namespace SharpBunny.Tests;

public class VideoServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly HttpClient _httpClient;
    private readonly VideoService _videoService;
    private readonly JsonSerializerOptions _jsonOptions;

    public VideoServiceTests()
    {
        _mockHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHandler.Object)
        {
            BaseAddress = new Uri("https://video.bunnycdn.com")
        };
        _videoService = new VideoService(_httpClient);
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    [Fact]
    public async Task GetVideoHeatmapAsync_WithValidVideoId_ReturnsHeatmapData()
    {
        // Arrange
        var libraryId = 123;
        var videoId = "test-video-id";
        var expectedResponse = new VideoHeatmapData
        {
            Heatmap = new List<VideoHeatmap>
            {
                new VideoHeatmap { TimeStamp = 10.5, Watchers = 100 },
                new VideoHeatmap { TimeStamp = 20.0, Watchers = 85 }
            }
        };

        var jsonContent = JsonSerializer.Serialize(expectedResponse, _jsonOptions);
        
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.RequestUri!.ToString().EndsWith($"/library/{libraryId}/videos/{videoId}/heatmap") &&
                    req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonContent)
            });

        // Act
        var result = await _videoService.GetVideoHeatmapAsync(libraryId, videoId);

        // Assert
        result.Should().NotBeNull();
        result.Heatmap.Should().HaveCount(2);
        result.Heatmap[0].TimeStamp.Should().Be(10.5);
        result.Heatmap[0].Watchers.Should().Be(100);
    }

    [Fact]
    public async Task GetVideoHeatmapAsync_WithEmptyVideoId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _videoService.GetVideoHeatmapAsync(123, string.Empty));
    }

    [Fact]
    public async Task GetVideoPlayDataAsync_WithDateRange_ReturnsPlayData()
    {
        // Arrange
        var libraryId = 123;
        var videoId = "test-video-id";
        var dateFrom = new DateTime(2023, 1, 1);
        var dateTo = new DateTime(2023, 1, 31);
        var expectedResponse = new List<VideoPlayData>
        {
            new VideoPlayData { Date = dateFrom, Views = 1000, WatchTime = 50000, Impressions = 1500 }
        };

        var jsonContent = JsonSerializer.Serialize(expectedResponse, _jsonOptions);
        
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.RequestUri!.ToString().Contains($"/library/{libraryId}/videos/{videoId}/play") &&
                    req.RequestUri.ToString().Contains("dateFrom=2023-01-01") &&
                    req.RequestUri.ToString().Contains("dateTo=2023-01-31") &&
                    req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonContent)
            });

        // Act
        var result = await _videoService.GetVideoPlayDataAsync(libraryId, videoId, dateFrom, dateTo);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].Views.Should().Be(1000);
        result[0].WatchTime.Should().Be(50000);
    }

    [Fact]
    public async Task GetVideoStatisticsAsync_WithValidVideoId_ReturnsStatistics()
    {
        // Arrange
        var libraryId = 123;
        var videoId = "test-video-id";
        var expectedResponse = new VideoStatistics
        {
            TotalViews = 5000,
            TotalWatchTime = 250000,
            AverageWatchTime = 50.0,
            Impressions = 7500,
            Clicks = 1000,
            ClickThroughRate = 0.133,
            Countries = new Dictionary<string, long> { { "US", 3000 }, { "UK", 2000 } },
            Devices = new Dictionary<string, long> { { "Desktop", 3500 }, { "Mobile", 1500 } },
            Referrers = new Dictionary<string, long> { { "Direct", 2500 }, { "Google", 2500 } }
        };

        var jsonContent = JsonSerializer.Serialize(expectedResponse, _jsonOptions);
        
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.RequestUri!.ToString().EndsWith($"/library/{libraryId}/videos/{videoId}/statistics") &&
                    req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonContent)
            });

        // Act
        var result = await _videoService.GetVideoStatisticsAsync(libraryId, videoId);

        // Assert
        result.Should().NotBeNull();
        result.TotalViews.Should().Be(5000);
        result.Countries.Should().ContainKey("US").WhoseValue.Should().Be(3000);
        result.Devices.Should().ContainKey("Desktop").WhoseValue.Should().Be(3500);
    }

    [Fact]
    public async Task GetVideoHeatmapDataAsync_WithValidVideoId_ReturnsHeatmapList()
    {
        // Arrange
        var libraryId = 123;
        var videoId = "test-video-id";
        var expectedResponse = new List<VideoHeatmap>
        {
            new VideoHeatmap { TimeStamp = 0.0, Watchers = 100 },
            new VideoHeatmap { TimeStamp = 10.0, Watchers = 95 },
            new VideoHeatmap { TimeStamp = 20.0, Watchers = 80 }
        };

        var jsonContent = JsonSerializer.Serialize(expectedResponse, _jsonOptions);
        
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.RequestUri!.ToString().EndsWith($"/library/{libraryId}/videos/{videoId}/heatmapdata") &&
                    req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonContent)
            });

        // Act
        var result = await _videoService.GetVideoHeatmapDataAsync(libraryId, videoId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result[0].Watchers.Should().Be(100);
        result[2].TimeStamp.Should().Be(20.0);
    }

    [Fact]
    public async Task ReencodeVideoAsync_WithValidVideoId_ReturnsReencodeResult()
    {
        // Arrange
        var libraryId = 123;
        var videoId = "test-video-id";
        var expectedResponse = new VideoReencodeResult
        {
            Success = true,
            StatusCode = 200,
            Message = "Video reencode started successfully"
        };

        var jsonContent = JsonSerializer.Serialize(expectedResponse, _jsonOptions);
        
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.RequestUri!.ToString().EndsWith($"/library/{libraryId}/videos/{videoId}/reencode") &&
                    req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonContent)
            });

        // Act
        var result = await _videoService.ReencodeVideoAsync(libraryId, videoId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(200);
        result.Message.Should().Be("Video reencode started successfully");
    }

    [Fact]
    public async Task ReencodeUsingCodecAsync_WithValidParameters_ReturnsReencodeResult()
    {
        // Arrange
        var libraryId = 123;
        var videoId = "test-video-id";
        var codec = "h264";
        var expectedResponse = new VideoReencodeResult
        {
            Success = true,
            StatusCode = 200,
            Message = "Video reencode with codec started successfully"
        };

        var jsonContent = JsonSerializer.Serialize(expectedResponse, _jsonOptions);
        
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.RequestUri!.ToString().EndsWith($"/library/{libraryId}/videos/{videoId}/reencode") &&
                    req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonContent)
            });

        // Act
        var result = await _videoService.ReencodeUsingCodecAsync(libraryId, videoId, codec);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("codec");
    }

    [Fact]
    public async Task ReencodeUsingCodecAsync_WithEmptyCodec_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _videoService.ReencodeUsingCodecAsync(123, "video-id", string.Empty));
    }

    [Fact]
    public async Task RepackageVideoAsync_WithValidVideoId_ReturnsRepackageResult()
    {
        // Arrange
        var libraryId = 123;
        var videoId = "test-video-id";
        var expectedResponse = new VideoReencodeResult
        {
            Success = true,
            StatusCode = 200,
            Message = "Video repackage started successfully"
        };

        var jsonContent = JsonSerializer.Serialize(expectedResponse, _jsonOptions);
        
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.RequestUri!.ToString().EndsWith($"/library/{libraryId}/videos/{videoId}/repackage") &&
                    req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonContent)
            });

        // Act
        var result = await _videoService.RepackageVideoAsync(libraryId, videoId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("repackage");
    }

    [Fact]
    public async Task SetThumbnailAsync_WithValidParameters_ReturnsUpdatedVideo()
    {
        // Arrange
        var libraryId = 123;
        var videoId = "test-video-id";
        var thumbnailUrl = "https://example.com/thumbnail.jpg";
        var expectedResponse = new Video
        {
            Guid = videoId,
            Title = "Test Video",
            ThumbnailFileName = "thumbnail.jpg"
        };

        var jsonContent = JsonSerializer.Serialize(expectedResponse, _jsonOptions);
        
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.RequestUri!.ToString().EndsWith($"/library/{libraryId}/videos/{videoId}/thumbnail") &&
                    req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonContent)
            });

        // Act
        var result = await _videoService.SetThumbnailAsync(libraryId, videoId, thumbnailUrl);

        // Assert
        result.Should().NotBeNull();
        result.Guid.Should().Be(videoId);
        result.ThumbnailFileName.Should().Be("thumbnail.jpg");
    }

    [Fact]
    public async Task SetThumbnailAsync_WithEmptyThumbnailUrl_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _videoService.SetThumbnailAsync(123, "video-id", string.Empty));
    }

    [Fact]
    public async Task AddCaptionAsync_WithValidParameters_ReturnsUpdatedVideo()
    {
        // Arrange
        var libraryId = 123;
        var videoId = "test-video-id";
        var srclang = "en";
        var label = "English";
        var captionContent = "WEBVTT\n\n00:00:00.000 --> 00:00:02.000\nHello World";
        var captionStream = new MemoryStream(Encoding.UTF8.GetBytes(captionContent));
        
        var expectedResponse = new Video
        {
            Guid = videoId,
            Title = "Test Video",
            Captions = new List<Caption>
            {
                new Caption { SourceLanguage = srclang, Label = label, CaptionsFileName = "en.vtt" }
            }
        };

        var jsonContent = JsonSerializer.Serialize(expectedResponse, _jsonOptions);
        
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.RequestUri!.ToString().EndsWith($"/library/{libraryId}/videos/{videoId}/captions/{srclang}") &&
                    req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonContent)
            });

        // Act
        var result = await _videoService.AddCaptionAsync(libraryId, videoId, srclang, label, captionStream);

        // Assert
        result.Should().NotBeNull();
        result.Captions.Should().HaveCount(1);
        result.Captions[0].SourceLanguage.Should().Be(srclang);
        result.Captions[0].Label.Should().Be(label);
    }

    [Fact]
    public async Task AddCaptionAsync_WithNullCaptionFile_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _videoService.AddCaptionAsync(123, "video-id", "en", "English", null!));
    }

    [Fact]
    public async Task DeleteCaptionAsync_WithValidParameters_CompletesSuccessfully()
    {
        // Arrange
        var libraryId = 123;
        var videoId = "test-video-id";
        var srclang = "en";
        
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.RequestUri!.ToString().EndsWith($"/library/{libraryId}/videos/{videoId}/captions/{srclang}") &&
                    req.Method == HttpMethod.Delete),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

        // Act & Assert (should not throw)
        await _videoService.DeleteCaptionAsync(libraryId, videoId, srclang);
    }

    [Fact]
    public async Task TranscribeVideoAsync_WithLanguage_ReturnsTranscribeResult()
    {
        // Arrange
        var libraryId = 123;
        var videoId = "test-video-id";
        var language = "en";
        var expectedResponse = new VideoReencodeResult
        {
            Success = true,
            StatusCode = 200,
            Message = "Video transcription started successfully"
        };

        var jsonContent = JsonSerializer.Serialize(expectedResponse, _jsonOptions);
        
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.RequestUri!.ToString().EndsWith($"/library/{libraryId}/videos/{videoId}/transcribe") &&
                    req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonContent)
            });

        // Act
        var result = await _videoService.TranscribeVideoAsync(libraryId, videoId, language);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("transcription");
    }

    [Fact]
    public async Task GetVideoResolutionsAsync_WithValidVideoId_ReturnsResolutions()
    {
        // Arrange
        var libraryId = 123;
        var videoId = "test-video-id";
        var expectedResponse = new List<VideoResolution>
        {
            new VideoResolution { Width = 1920, Height = 1080, Bitrate = 5000, Framerate = 30.0, FileSize = 1024000, FileName = "1080p.mp4" },
            new VideoResolution { Width = 1280, Height = 720, Bitrate = 2500, Framerate = 30.0, FileSize = 512000, FileName = "720p.mp4" }
        };

        var jsonContent = JsonSerializer.Serialize(expectedResponse, _jsonOptions);
        
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.RequestUri!.ToString().EndsWith($"/library/{libraryId}/videos/{videoId}/resolutions") &&
                    req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonContent)
            });

        // Act
        var result = await _videoService.GetVideoResolutionsAsync(libraryId, videoId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Width.Should().Be(1920);
        result[0].Height.Should().Be(1080);
        result[1].Width.Should().Be(1280);
        result[1].Height.Should().Be(720);
    }

    [Fact]
    public async Task DeleteResolutionsAsync_WithValidParameters_CompletesSuccessfully()
    {
        // Arrange
        var libraryId = 123;
        var videoId = "test-video-id";
        var resolutions = new List<string> { "720p", "480p" };
        
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.RequestUri!.ToString().EndsWith($"/library/{libraryId}/videos/{videoId}/resolutions") &&
                    req.Method == HttpMethod.Delete),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

        // Act & Assert (should not throw)
        await _videoService.DeleteResolutionsAsync(libraryId, videoId, resolutions);
    }

    [Fact]
    public async Task DeleteResolutionsAsync_WithEmptyResolutionsList_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _videoService.DeleteResolutionsAsync(123, "video-id", new List<string>()));
    }

    [Fact]
    public async Task DeleteResolutionsAsync_WithNullResolutionsList_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _videoService.DeleteResolutionsAsync(123, "video-id", null!));
    }
}
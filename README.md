# SharpBunny
.NET 8.x client implementation for the Bunny.net APIs. Full disclosure, I wanted to try out github co-pilot, and this seemed like a useful project. I am reviewing what co-pilot writes and updating anything I find broken or not correct.

## Overview

SharpBunny is a comprehensive .NET client library for interacting with Bunny.net APIs including Stream API, Edge Storage API, and General API. It provides a type-safe, async-first approach to managing video collections, videos, file storage, and CDN resources in your Bunny.net services.

## Features

### Stream API
- ✅ **Collection Management**: Create, read, update, and delete video collections
- ✅ **Video Management**: Manage videos including upload, metadata updates, and deletion
- ✅ **Type-Safe Models**: Strongly-typed models for all API responses
- ✅ **Async/Await Support**: Full async support for all operations
- ✅ **Error Handling**: Comprehensive error handling with custom exceptions
- ✅ **HTTP Client Integration**: Built on HttpClient with support for dependency injection
- ✅ **.NET 8 Compatible**: Built for .NET 8.0 with modern C# features

### Edge Storage API
- ✅ **File Management**: Upload, download, delete files and browse directories
- ✅ **Path Support**: Full support for nested directory structures
- ✅ **Checksum Validation**: SHA256 checksum support for upload integrity
- ✅ **Multiple Storage Regions**: Support for different storage zone endpoints
- ✅ **Type-Safe File Metadata**: Strongly-typed models for file information

### General API
- ✅ **Countries**: Get country information including tax rates and EU status
- ✅ **Regions**: Retrieve CDN region data with pricing tiers
- ✅ **Pull Zones**: Full CDN management including creation, configuration, and deletion
- ✅ **DNS Zones**: DNS zone management with record operations
- ✅ **Purge**: Cache purging for URLs and entire pull zones
- ✅ **Statistics**: Comprehensive analytics for pull zones and billing data

## Installation

```bash
dotnet add package SharpBunny
```

## Quick Start

### General API
```csharp
using SharpBunny;

// Initialize the General API client
var generalApi = new BunnyGeneralApi("your-general-api-key");

// Get all countries
var countries = await generalApi.Countries.GetCountriesAsync();

foreach (var country in countries)
{
    Console.WriteLine($"Country: {country.Name} ({country.Code}) - EU: {country.IsEU}");
}

// Get all pull zones
var pullZones = await generalApi.PullZones.GetPullZonesAsync();

foreach (var pullZone in pullZones.Items)
{
    Console.WriteLine($"Pull Zone: {pullZone.Name} - Origin: {pullZone.OriginUrl}");
    Console.WriteLine($"Status: {(pullZone.Enabled ? "Enabled" : "Disabled")}");
}

// Create a new pull zone
var createRequest = new CreatePullZoneRequest
{
    Name = "my-new-pull-zone",
    OriginUrl = "https://myorigin.example.com",
    Type = 0 // Standard pull zone
};
var newPullZone = await generalApi.PullZones.CreatePullZoneAsync(createRequest);

// Get pull zone statistics
var stats = await generalApi.Statistics.GetPullZoneStatisticsAsync(
    newPullZone.Id,
    dateFrom: DateTime.UtcNow.AddDays(-30),
    dateTo: DateTime.UtcNow);

Console.WriteLine($"Bandwidth used: {stats.TotalBandwidthUsed:N0} bytes");
Console.WriteLine($"Requests served: {stats.TotalRequestsServed:N0}");
Console.WriteLine($"Cache hit rate: {stats.CacheHitRate:P2}");
```

### Stream API
```csharp
using SharpBunny;

// Initialize the Stream API client
var streamApi = new BunnyStreamApi("your-stream-api-key");

// Get all collections for a library
var libraryId = 12345;
var collections = await streamApi.Collections.GetCollectionsAsync(libraryId);

foreach (var collection in collections.Items)
{
    Console.WriteLine($"Collection: {collection.Name} ({collection.VideoCount} videos)");
}

// Create a new collection
var newCollection = await streamApi.Collections.CreateCollectionAsync(libraryId, "My New Collection");
Console.WriteLine($"Created collection: {newCollection.Guid}");

// Get all videos in the library
var videos = await streamApi.Videos.GetVideosAsync(libraryId);

foreach (var video in videos.Items)
{
    Console.WriteLine($"Video: {video.Title} - Views: {video.Views}");
}
```

### Edge Storage API
```csharp
using SharpBunny;
using System.Text;

// Initialize the Edge Storage API client
var edgeStorageApi = new BunnyEdgeStorageApi("your-storage-zone-password");

// List files in the root directory
var files = await edgeStorageApi.EdgeStorage.ListFilesAsync(
    "your-storage-zone",
    storageZonePassword: "your-storage-zone-password");

foreach (var file in files)
{
    var type = file.IsDirectory ? "DIR" : "FILE";
    Console.WriteLine($"{type}: {file.ObjectName} ({file.Length} bytes)");
}

// Upload a file
var content = Encoding.UTF8.GetBytes("Hello, World!");
await edgeStorageApi.EdgeStorage.UploadFileAsync(
    "your-storage-zone",
    "hello.txt",
    content,
    path: "my-folder",
    storageZonePassword: "your-storage-zone-password");

// Download a file
var downloadedContent = await edgeStorageApi.EdgeStorage.DownloadFileAsync(
    "your-storage-zone",
    "hello.txt",
    path: "my-folder",
    storageZonePassword: "your-storage-zone-password");

var text = Encoding.UTF8.GetString(downloadedContent);
Console.WriteLine($"Downloaded: {text}");

// Delete a file
await edgeStorageApi.EdgeStorage.DeleteFileAsync(
    "your-storage-zone",
    "hello.txt",
    path: "my-folder",
    storageZonePassword: "your-storage-zone-password");
```

## API Reference

### BunnyGeneralApi

The main entry point for the General API client.

#### Constructor
```csharp
// With automatic HttpClient management
var api = new BunnyGeneralApi("your-general-api-key");

// With custom HttpClient (for DI scenarios)
var api = new BunnyGeneralApi(httpClient, "your-general-api-key");
```

### BunnyStreamApi

The main entry point for the Stream API client.

#### Constructor
```csharp
// With automatic HttpClient management
var api = new BunnyStreamApi("your-stream-api-key");

// With custom HttpClient (for DI scenarios)
var api = new BunnyStreamApi(httpClient, "your-stream-api-key");
```

### BunnyEdgeStorageApi

The main entry point for the Edge Storage API client.

#### Constructor
```csharp
// With automatic HttpClient management
var api = new BunnyEdgeStorageApi("your-storage-zone-password");

// With custom HttpClient (for DI scenarios)
var api = new BunnyEdgeStorageApi(httpClient, "your-storage-zone-password");
```

### Collection Management

#### Get Collections
```csharp
var collections = await api.Collections.GetCollectionsAsync(
    libraryId: 12345,
    page: 1,
    itemsPerPage: 100,
    search: "search term", // optional
    orderBy: "date" // optional
);
```

#### Get Single Collection
```csharp
var collection = await api.Collections.GetCollectionAsync(libraryId, collectionId);
```

#### Create Collection
```csharp
var newCollection = await api.Collections.CreateCollectionAsync(libraryId, "Collection Name");
```

#### Update Collection
```csharp
var updatedCollection = await api.Collections.UpdateCollectionAsync(
    libraryId, 
    collectionId, 
    "New Collection Name"
);
```

#### Delete Collection
```csharp
await api.Collections.DeleteCollectionAsync(libraryId, collectionId);
```

### Video Management

#### Get Videos
```csharp
var videos = await api.Videos.GetVideosAsync(
    libraryId: 12345,
    page: 1,
    itemsPerPage: 100,
    search: "search term", // optional
    collection: "collection-id", // optional
    orderBy: "date" // optional
);
```

#### Get Single Video
```csharp
var video = await api.Videos.GetVideoAsync(libraryId, videoId);
```

#### Create Video
```csharp
var newVideo = await api.Videos.CreateVideoAsync(
    libraryId, 
    "Video Title",
    collectionId // optional
);
```

#### Update Video
```csharp
var updatedVideo = await api.Videos.UpdateVideoAsync(
    libraryId,
    videoId,
    title: "New Title", // optional
    collectionId: "new-collection-id" // optional
);
```

#### Delete Video
```csharp
await api.Videos.DeleteVideoAsync(libraryId, videoId);
```

#### Upload Video
```csharp
using var fileStream = File.OpenRead("video.mp4");
var success = await api.Videos.UploadVideoAsync(
    libraryId,
    videoId,
    fileStream,
    "video.mp4"
);
```

### General API Services

#### Countries Service
```csharp
// Get all countries
var countries = await api.Countries.GetCountriesAsync();
```

#### Regions Service
```csharp
// Get all regions
var regions = await api.Regions.GetRegionsAsync();
```

#### Pull Zones Service
```csharp
// Get all pull zones
var pullZones = await api.PullZones.GetPullZonesAsync(
    page: 1,
    perPage: 100,
    search: "example" // optional
);

// Get specific pull zone
var pullZone = await api.PullZones.GetPullZoneAsync(pullZoneId);

// Create pull zone
var createRequest = new CreatePullZoneRequest
{
    Name = "my-pull-zone",
    OriginUrl = "https://origin.example.com",
    Type = 0 // 0 = Standard, 1 = Volume
};
var newPullZone = await api.PullZones.CreatePullZoneAsync(createRequest);

// Update pull zone
var updateRequest = new UpdatePullZoneRequest
{
    OriginUrl = "https://neworigin.example.com",
    EnableLogging = true,
    CacheControlMaxAgeOverride = 3600
};
var updatedPullZone = await api.PullZones.UpdatePullZoneAsync(pullZoneId, updateRequest);

// Delete pull zone
await api.PullZones.DeletePullZoneAsync(pullZoneId);
```

#### DNS Zones Service
```csharp
// Get all DNS zones
var dnsZones = await api.DnsZones.GetDnsZonesAsync(
    page: 1,
    perPage: 100,
    search: "example.com" // optional
);

// Get specific DNS zone
var dnsZone = await api.DnsZones.GetDnsZoneAsync(zoneId);

// Create DNS zone
var newDnsZone = await api.DnsZones.CreateDnsZoneAsync("example.com");

// Delete DNS zone
await api.DnsZones.DeleteDnsZoneAsync(zoneId);
```

#### Purge Service
```csharp
// Purge entire pull zone cache
await api.Purge.PurgePullZoneCacheAsync(pullZoneId);

// Purge specific URLs
var purgeRequest = new PurgeUrlsRequest
{
    Urls = new List<string> 
    { 
        "https://example.com/image.jpg",
        "https://example.com/style.css"
    },
    Async = false
};
await api.Purge.PurgeUrlsAsync(purgeRequest);

// Get purge history
var purgeHistory = await api.Purge.GetPurgeHistoryAsync(page: 1, perPage: 100);
```

#### Statistics Service
```csharp
// Get pull zone statistics
var stats = await api.Statistics.GetPullZoneStatisticsAsync(
    pullZoneId,
    dateFrom: DateTime.UtcNow.AddDays(-30),
    dateTo: DateTime.UtcNow,
    hourly: false,
    loadErrors: false
);

// Get billing statistics
var billingStats = await api.Statistics.GetBillingStatisticsAsync(
    dateFrom: DateTime.UtcNow.AddMonths(-1),
    dateTo: DateTime.UtcNow
);

// Get country statistics for a pull zone
var countryStats = await api.Statistics.GetPullZoneCountryStatisticsAsync(
    pullZoneId,
    dateFrom: DateTime.UtcNow.AddDays(-7),
    dateTo: DateTime.UtcNow
);
```

### Edge Storage API

#### List Files
```csharp
var files = await api.EdgeStorage.ListFilesAsync(
    storageZoneName: "your-storage-zone",
    path: "subfolder", // optional, defaults to root
    storageZonePassword: "your-password", // optional if set in constructor
    storageZoneEndpoint: "ny.storage.bunnycdn.com", // optional, defaults to storage.bunnycdn.com
    cancellationToken: cancellationToken // optional
);
```

#### Upload File
```csharp
var fileContent = File.ReadAllBytes("local-file.jpg");
await api.EdgeStorage.UploadFileAsync(
    storageZoneName: "your-storage-zone",
    fileName: "uploaded-file.jpg",
    fileContent: fileContent,
    path: "uploads", // optional
    storageZonePassword: "your-password", // optional if set in constructor
    storageZoneEndpoint: "ny.storage.bunnycdn.com", // optional
    checksum: "sha256-checksum", // optional for integrity validation
    cancellationToken: cancellationToken // optional
);
```

#### Download File
```csharp
var fileContent = await api.EdgeStorage.DownloadFileAsync(
    storageZoneName: "your-storage-zone",
    fileName: "file-to-download.jpg",
    path: "uploads", // optional
    storageZonePassword: "your-password", // optional if set in constructor
    storageZoneEndpoint: "ny.storage.bunnycdn.com", // optional
    cancellationToken: cancellationToken // optional
);

// Save to local file
await File.WriteAllBytesAsync("downloaded-file.jpg", fileContent);
```

#### Delete File
```csharp
await api.EdgeStorage.DeleteFileAsync(
    storageZoneName: "your-storage-zone",
    fileName: "file-to-delete.jpg",
    path: "uploads", // optional
    storageZonePassword: "your-password", // optional if set in constructor
    storageZoneEndpoint: "ny.storage.bunnycdn.com", // optional
    cancellationToken: cancellationToken // optional
);
```

## Error Handling

The library throws `BunnyApiException` for API errors:

```csharp
try
{
    var collection = await api.Collections.GetCollectionAsync(libraryId, "invalid-id");
}
catch (BunnyApiException ex)
{
    Console.WriteLine($"API Error: {ex.Message}");
    Console.WriteLine($"Status Code: {ex.StatusCode}");
    
    if (ex.ApiError != null)
    {
        Console.WriteLine($"Error Type: {ex.ApiError.Type}");
        Console.WriteLine($"Error Detail: {ex.ApiError.Detail}");
    }
}
```

## Dependency Injection

You can register both API clients with the .NET dependency injection container:

```csharp
// In Program.cs or Startup.cs
services.AddHttpClient();

// Stream API
services.AddSingleton(provider =>
{
    var httpClient = provider.GetRequiredService<HttpClient>();
    return new BunnyStreamApi(httpClient, "your-stream-api-key");
});

// General API
services.AddSingleton(provider =>
{
    var httpClient = provider.GetRequiredService<HttpClient>();
    return new BunnyGeneralApi(httpClient, "your-general-api-key");
});

// Edge Storage API
services.AddSingleton(provider =>
{
    var httpClient = provider.GetRequiredService<HttpClient>();
    return new BunnyEdgeStorageApi(httpClient, "your-storage-zone-password");
});
```

## Models

### General API Models

#### Country
- `Id`: Unique country identifier
- `Name`: Country name
- `Code`: ISO country code (e.g., "US", "DE")
- `ContinentCode`: Continent code (e.g., "NA", "EU")
- `IsEU`: Whether the country is in the European Union
- `TaxRate`: Tax rate as decimal (e.g., 19 for 19%)

#### Region
- `Id`: Unique region identifier
- `Name`: Region name
- `RegionCode`: Region code
- `ContinentCode`: Continent code
- `CountryCode`: Country code
- `Latitude`/`Longitude`: Geographic coordinates
- `PriceTier`: Pricing tier level
- `RegionPrice`: Price per unit in this region

#### PullZone
- `Id`: Unique pull zone identifier
- `Name`: Pull zone name
- `OriginUrl`: Origin server URL
- `Enabled`: Whether the pull zone is active
- `Hostnames`: List of custom hostnames
- `StorageZoneId`: Associated storage zone ID (if any)
- `ZonePricingTier`: Pricing tier (0 = Standard, 1 = High Volume)
- `MonthlyBandwidthLimit`/`MonthlyBandwidthUsed`: Bandwidth limits and usage
- `MonthlyCharges`: Current month charges
- `Type`: Pull zone type (0 = Standard, 1 = Volume)
- Various caching, security, and logging configuration options

#### DnsZone
- `Id`: Unique DNS zone identifier
- `Domain`: Domain name
- `RecordsCount`: Number of DNS records
- `DateCreated`/`DateModified`: Timestamps
- `Nameservers`: List of nameservers
- `CustomNameservers`: Custom nameserver configuration
- `SoaEmail`: SOA record email
- Various logging and forwarding configuration options

### Stream API Models

#### Collection
- `VideoLibraryId`: The video library ID
- `Guid`: Unique collection identifier
- `Name`: Collection name
- `VideoCount`: Number of videos in the collection
- `TotalSize`: Total size of all videos in bytes
- `PreviewVideoIds`: Comma-separated list of preview video IDs

### Video
- `VideoLibraryId`: The video library ID
- `Guid`: Unique video identifier
- `Title`: Video title
- `DateUploaded`: Upload timestamp
- `Views`: View count
- `IsPublic`: Public visibility flag
- `Length`: Video duration in seconds
- `Status`: Processing status
- `Width`/`Height`: Video dimensions
- `StorageSize`: File size in bytes
- `CollectionId`: Associated collection ID
- `Captions`: List of caption tracks
- `Chapters`: List of video chapters
- `MetaTags`: List of metadata tags

### Edge Storage API Models

#### StorageFile
- `ArrayNumber`: Array position number
- `Checksum`: SHA256 checksum (null for directories)
- `ContentType`: MIME type of the file
- `DateCreated`: ISO 8601 creation timestamp
- `Guid`: Unique file identifier
- `IsDirectory`: Boolean indicating if item is a directory
- `LastChanged`: ISO 8601 last modified timestamp
- `Length`: File size in bytes (0 for directories)
- `ObjectName`: File or directory name
- `Path`: Full path to the file
- `ReplicatedZones`: Storage zone replication regions
- `ServerId`: Server ID where file is stored
- `StorageZoneId`: Storage zone identifier
- `StorageZoneName`: Storage zone name
- `UserId`: User ID associated with the file

## Contributing

This library implements the core Bunny.net Stream API, Edge Storage API, and General API endpoints. If you need additional functionality or find bugs, please open an issue or submit a pull request.

## License

This project is licensed under the MIT License.

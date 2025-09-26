# SharpBunny
.NET 8.x client implementation for the Bunny.net Stream API and Edge Storage API

## Overview

SharpBunny is a comprehensive .NET client library for interacting with both the Bunny.net Stream API and Edge Storage API. It provides a type-safe, async-first approach to managing video collections, videos, and file storage in your Bunny.net services.

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

## Installation

```bash
dotnet add package SharpBunny
```

## Quick Start

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

// Edge Storage API
services.AddSingleton(provider =>
{
    var httpClient = provider.GetRequiredService<HttpClient>();
    return new BunnyEdgeStorageApi(httpClient, "your-storage-zone-password");
});
```

## Models

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

This library implements the core Bunny.net Stream API and Edge Storage API endpoints. If you need additional functionality or find bugs, please open an issue or submit a pull request.

## License

This project is licensed under the MIT License.

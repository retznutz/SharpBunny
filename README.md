# SharpBunny
.NET 8.x client implementation for the Bunny.net Stream API

## Overview

SharpBunny is a comprehensive .NET client library for interacting with the Bunny.net Stream API. It provides a type-safe, async-first approach to managing video collections and videos in your Bunny.net video library.

## Features

- ✅ **Collection Management**: Create, read, update, and delete video collections
- ✅ **Video Management**: Manage videos including upload, metadata updates, and deletion
- ✅ **Type-Safe Models**: Strongly-typed models for all API responses
- ✅ **Async/Await Support**: Full async support for all operations
- ✅ **Error Handling**: Comprehensive error handling with custom exceptions
- ✅ **HTTP Client Integration**: Built on HttpClient with support for dependency injection
- ✅ **.NET 8 Compatible**: Built for .NET 8.0 with modern C# features

## Installation

```bash
dotnet add package SharpBunny
```

## Quick Start

```csharp
using SharpBunny;

// Initialize the API client
var api = new BunnyStreamApi("your-api-key-here");

// Get all collections for a library
var libraryId = 12345;
var collections = await api.Collections.GetCollectionsAsync(libraryId);

foreach (var collection in collections.Items)
{
    Console.WriteLine($"Collection: {collection.Name} ({collection.VideoCount} videos)");
}

// Create a new collection
var newCollection = await api.Collections.CreateCollectionAsync(libraryId, "My New Collection");
Console.WriteLine($"Created collection: {newCollection.Guid}");

// Get all videos in the library
var videos = await api.Videos.GetVideosAsync(libraryId);

foreach (var video in videos.Items)
{
    Console.WriteLine($"Video: {video.Title} - Views: {video.Views}");
}

// Create a new video
var newVideo = await api.Videos.CreateVideoAsync(libraryId, "My New Video", newCollection.Guid);
Console.WriteLine($"Created video: {newVideo.Guid}");
```

## API Reference

### BunnyStreamApi

The main entry point for the API client.

#### Constructor
```csharp
// With automatic HttpClient management
var api = new BunnyStreamApi("your-api-key");

// With custom HttpClient (for DI scenarios)
var api = new BunnyStreamApi(httpClient, "your-api-key");
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

You can register the API client with the .NET dependency injection container:

```csharp
// In Program.cs or Startup.cs
services.AddHttpClient();
services.AddSingleton(provider =>
{
    var httpClient = provider.GetRequiredService<HttpClient>();
    return new BunnyStreamApi(httpClient, "your-api-key");
});
```

## Models

### Collection
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

## Contributing

This library implements the core Bunny.net Stream API endpoints. If you need additional functionality or find bugs, please open an issue or submit a pull request.

## License

This project is licensed under the MIT License.

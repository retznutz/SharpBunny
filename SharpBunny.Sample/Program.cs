using SharpBunny;

// Sample program demonstrating SharpBunny usage
Console.WriteLine("SharpBunny - Bunny.net Stream API Client Sample");
Console.WriteLine("===============================================");

// Note: In a real application, store your API key securely (environment variables, Azure Key Vault, etc.)
const string apiKey = "your-api-key-here";
const int libraryId = 12345; // Your video library ID

try
{
    // Initialize the API client
    var api = new BunnyStreamApi(apiKey);
    Console.WriteLine("✓ API client initialized");

    Console.WriteLine("\n--- Collection Management ---");
    
    // Get all collections
    Console.WriteLine("Fetching collections...");
    var collections = await api.Collections.GetCollectionsAsync(libraryId, page: 1, itemsPerPage: 10);
    
    Console.WriteLine($"Found {collections.TotalItems} collections (showing {collections.Items.Count}):");
    foreach (var collection in collections.Items)
    {
        Console.WriteLine($"  • {collection.Name} ({collection.VideoCount} videos, {collection.TotalSize:N0} bytes)");
    }

    // Create a new collection (commented out to avoid creating test data)
    /*
    Console.WriteLine("\nCreating new collection...");
    var newCollection = await api.Collections.CreateCollectionAsync(libraryId, "Sample Collection from SharpBunny");
    Console.WriteLine($"✓ Created collection: {newCollection.Name} (ID: {newCollection.Guid})");
    */

    Console.WriteLine("\n--- Video Management ---");
    
    // Get videos
    Console.WriteLine("Fetching videos...");
    var videos = await api.Videos.GetVideosAsync(libraryId, page: 1, itemsPerPage: 10);
    
    Console.WriteLine($"Found {videos.TotalItems} videos (showing {videos.Items.Count}):");
    foreach (var video in videos.Items)
    {
        var status = video.Status switch
        {
            0 => "Created",
            1 => "Uploaded", 
            2 => "Processing",
            3 => "Finished",
            4 => "Error",
            _ => $"Status {video.Status}"
        };
        
        Console.WriteLine($"  • {video.Title}");
        Console.WriteLine($"    Views: {video.Views:N0} | Duration: {TimeSpan.FromSeconds(video.Length)} | Status: {status}");
        Console.WriteLine($"    Size: {video.StorageSize:N0} bytes | Resolution: {video.Width}x{video.Height}");
        
        if (!string.IsNullOrEmpty(video.CollectionId))
        {
            Console.WriteLine($"    Collection: {video.CollectionId}");
        }
        Console.WriteLine();
    }

    // Example of searching videos
    Console.WriteLine("Searching for videos containing 'sample'...");
    var searchResults = await api.Videos.GetVideosAsync(libraryId, search: "sample", itemsPerPage: 5);
    Console.WriteLine($"Found {searchResults.TotalItems} videos matching 'sample'");

    Console.WriteLine("\n--- Error Handling Example ---");
    try
    {
        // This will likely fail with a 404 error
        await api.Collections.GetCollectionAsync(libraryId, "non-existent-collection-id");
    }
    catch (BunnyApiException ex)
    {
        Console.WriteLine($"Expected API error caught: {ex.Message}");
        Console.WriteLine($"Status Code: {ex.StatusCode}");
        if (ex.ApiError != null)
        {
            Console.WriteLine($"Error Type: {ex.ApiError.Type}");
        }
    }

    Console.WriteLine("\n✓ Sample completed successfully!");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"❌ Configuration error: {ex.Message}");
    Console.WriteLine("Please update the apiKey and libraryId constants in Program.cs with your actual values.");
}
catch (BunnyApiException ex)
{
    Console.WriteLine($"❌ API error: {ex.Message}");
    Console.WriteLine($"Status Code: {ex.StatusCode}");
    
    if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
    {
        Console.WriteLine("Check that your API key is correct and has the necessary permissions.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Unexpected error: {ex.Message}");
    Console.WriteLine($"Type: {ex.GetType().Name}");
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();

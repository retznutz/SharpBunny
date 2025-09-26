using SharpBunny;
using System.Text;

// Sample program demonstrating SharpBunny usage
Console.WriteLine("SharpBunny - Bunny.net API Client Sample");
Console.WriteLine("=========================================");

// Note: In a real application, store your API keys securely (environment variables, Azure Key Vault, etc.)
const string streamApiKey = "your-stream-api-key-here";
const string storageZonePassword = "your-storage-zone-password-here";
const string storageZoneName = "your-storage-zone-name";
const string generalApiKey = "your-general-api-key-here";
const int libraryId = 12345; // Your video library ID

try
{
    Console.WriteLine("\n=== Stream API Examples ===");
    
    // Initialize the Stream API client
    var streamApi = new BunnyStreamApi(streamApiKey);
    Console.WriteLine("✓ Stream API client initialized");

    Console.WriteLine("\n--- Collection Management ---");
    
    // Get all collections
    Console.WriteLine("Fetching collections...");
    var collections = await streamApi.Collections.GetCollectionsAsync(libraryId, page: 1, itemsPerPage: 10);
    
    Console.WriteLine($"Found {collections.TotalItems} collections (showing {collections.Items.Count}):");
    foreach (var collection in collections.Items)
    {
        Console.WriteLine($"  • {collection.Name} ({collection.VideoCount} videos, {collection.TotalSize:N0} bytes)");
    }

    // Create a new collection (commented out to avoid creating test data)
    /*
    Console.WriteLine("\nCreating new collection...");
    var newCollection = await streamApi.Collections.CreateCollectionAsync(libraryId, "Sample Collection from SharpBunny");
    Console.WriteLine($"✓ Created collection: {newCollection.Name} (ID: {newCollection.Guid})");
    */

    Console.WriteLine("\n--- Video Management ---");
    
    // Get videos
    Console.WriteLine("Fetching videos...");
    var videos = await streamApi.Videos.GetVideosAsync(libraryId, page: 1, itemsPerPage: 10);
    
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

    Console.WriteLine("\n=== Edge Storage API Examples ===");
    
    // Initialize the Edge Storage API client
    var edgeStorageApi = new BunnyEdgeStorageApi(storageZonePassword);
    Console.WriteLine("✓ Edge Storage API client initialized");

    Console.WriteLine("\n--- File Management ---");
    
    // List files in the root directory
    Console.WriteLine("Listing files in root directory...");
    var files = await edgeStorageApi.EdgeStorage.ListFilesAsync(
        storageZoneName,
        storageZonePassword: storageZonePassword);
    
    Console.WriteLine($"Found {files.Count} items in root directory:");
    foreach (var file in files)
    {
        var type = file.IsDirectory ? "📁 DIR " : "📄 FILE";
        var size = file.IsDirectory ? "" : $" ({file.Length:N0} bytes)";
        Console.WriteLine($"  {type} {file.ObjectName}{size}");
        if (!file.IsDirectory && !string.IsNullOrEmpty(file.ContentType))
        {
            Console.WriteLine($"       Content-Type: {file.ContentType}");
        }
    }

    // Example of uploading a small text file (commented out to avoid creating test data)
    /*
    Console.WriteLine("\nUploading sample file...");
    var sampleContent = Encoding.UTF8.GetBytes("Hello from SharpBunny Edge Storage API! 🚀");
    await edgeStorageApi.EdgeStorage.UploadFileAsync(
        storageZoneName,
        "sample-file.txt",
        sampleContent,
        path: "test-folder",
        storageZonePassword: storageZonePassword);
    Console.WriteLine("✓ File uploaded successfully");

    Console.WriteLine("\nDownloading the file back...");
    var downloadedContent = await edgeStorageApi.EdgeStorage.DownloadFileAsync(
        storageZoneName,
        "sample-file.txt",
        path: "test-folder",
        storageZonePassword: storageZonePassword);
    
    var downloadedText = Encoding.UTF8.GetString(downloadedContent);
    Console.WriteLine($"Downloaded content: {downloadedText}");

    Console.WriteLine("\nDeleting the sample file...");
    await edgeStorageApi.EdgeStorage.DeleteFileAsync(
        storageZoneName,
        "sample-file.txt",
        path: "test-folder",
        storageZonePassword: storageZonePassword);
    Console.WriteLine("✓ File deleted successfully");
    */

    Console.WriteLine("\n=== General API Examples ===");
    
    // Initialize the General API client  
    var generalApi = new BunnyGeneralApi(generalApiKey);
    Console.WriteLine("✓ General API client initialized");

    Console.WriteLine("\n--- Countries ---");
    
    // Get all countries
    Console.WriteLine("Fetching countries...");
    var countries = await generalApi.Countries.GetCountriesAsync();
    
    Console.WriteLine($"Found {countries.Count} countries (showing first 10):");
    foreach (var country in countries.Take(10))
    {
        var euStatus = country.IsEU ? "EU" : "Non-EU";
        Console.WriteLine($"  • {country.Name} ({country.Code}) - {euStatus} - Tax Rate: {country.TaxRate}%");
    }

    Console.WriteLine("\n--- Regions ---");
    
    // Get all regions
    Console.WriteLine("Fetching regions...");
    var regions = await generalApi.Regions.GetRegionsAsync();
    
    Console.WriteLine($"Found {regions.Count} regions (showing first 10):");
    foreach (var region in regions.Take(10))
    {
        Console.WriteLine($"  • {region.Name} ({region.RegionCode}) - {region.CountryCode} - Tier {region.PriceTier}");
    }

    Console.WriteLine("\n--- Pull Zones ---");
    
    // Get pull zones
    Console.WriteLine("Fetching pull zones...");
    var pullZones = await generalApi.PullZones.GetPullZonesAsync(page: 1, perPage: 10);
    
    Console.WriteLine($"Found {pullZones.TotalItems} pull zones (showing {pullZones.Items.Count}):");
    foreach (var pullZone in pullZones.Items)
    {
        var status = pullZone.Enabled ? "Enabled" : "Disabled";
        Console.WriteLine($"  • {pullZone.Name} - {status}");
        Console.WriteLine($"    Origin: {pullZone.OriginUrl}");
        Console.WriteLine($"    Bandwidth: {pullZone.MonthlyBandwidthUsed:N0} / {pullZone.MonthlyBandwidthLimit:N0} bytes");
        Console.WriteLine($"    Hostnames: {pullZone.Hostnames.Count}");
        Console.WriteLine();
    }

    Console.WriteLine("\n--- DNS Zones ---");
    
    // Get DNS zones
    Console.WriteLine("Fetching DNS zones...");
    var dnsZones = await generalApi.DnsZones.GetDnsZonesAsync(page: 1, perPage: 10);
    
    Console.WriteLine($"Found {dnsZones.TotalItems} DNS zones (showing {dnsZones.Items.Count}):");
    foreach (var dnsZone in dnsZones.Items)
    {
        Console.WriteLine($"  • {dnsZone.Domain} ({dnsZone.RecordsCount} records)");
        Console.WriteLine($"    Created: {dnsZone.DateCreated:yyyy-MM-dd}");
        Console.WriteLine($"    Nameservers: {string.Join(", ", dnsZone.Nameservers.Take(2))}");
        Console.WriteLine();
    }

    Console.WriteLine("\n--- Error Handling Example ---");
    try
    {
        // This will likely fail with a 404 error
        await streamApi.Collections.GetCollectionAsync(libraryId, "non-existent-collection-id");
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
    Console.WriteLine("Please update the API keys and configuration constants in Program.cs with your actual values.");
}
catch (BunnyApiException ex)
{
    Console.WriteLine($"❌ API error: {ex.Message}");
    Console.WriteLine($"Status Code: {ex.StatusCode}");
    
    if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
    {
        Console.WriteLine("Check that your API keys are correct and have the necessary permissions.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Unexpected error: {ex.Message}");
    Console.WriteLine($"Type: {ex.GetType().Name}");
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();

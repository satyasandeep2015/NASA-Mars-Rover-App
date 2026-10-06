using NasaMarsRoverConsoleApp.Services;
using Microsoft.Extensions.Configuration;

//Reads the configuration from the appsettins.json
var configuration = new ConfigurationBuilder()
    .AddJsonFile(
        Path.Combine(AppContext.BaseDirectory, "appsettings.json"),
        optional: false)
    .Build();

string apiKey = configuration["NasaApi:ApiKey"]?? string.Empty;
string baseUrl = configuration["NasaApi:BaseUrl"] ?? string.Empty;


//Reads input from the dates.txt file
var inputFile = args.Length > 0
    ? args[0]
    : Path.Combine(AppContext.BaseDirectory, "dates.txt");

//Path to save the photos
var photosDirectory = Path.Combine(Directory.GetCurrentDirectory(), "photos");

using var nasaHttpClient = new HttpClient
{
    BaseAddress = new Uri(baseUrl)
};

using var imageHttpClient = new HttpClient();

var processor = new RoverPhotoProcessor(
    new DateParser(),
    new NasaMarsRoverClient(nasaHttpClient, apiKey, baseUrl),
    new PhotoDownloader(imageHttpClient, photosDirectory));

try
{
    //Calls end point and tries to download photos related to all the dates in the input file
    var results = await processor.ProcessFileAsync(inputFile);

    Console.WriteLine("NASA Mars Rover Photo Download Summary");
    Console.WriteLine(new string('-', 72));

    foreach (var result in results)
    {
        var displayDate = result.Date?.ToString("yyyy-MM-dd") ?? result.Input;
        Console.WriteLine($"Date: {displayDate}");
        Console.WriteLine($"Photos found: {result.PhotosFound}");
        Console.WriteLine($"Downloaded: {result.Downloaded}");
        Console.WriteLine($"Skipped existing: {result.Skipped}");

        if (result.Errors.Count > 0)
        {
            foreach (var error in result.Errors)
                Console.WriteLine($"Error: {error}");
        }
        else
        {
            Console.WriteLine("Status: Success");
        }

        Console.WriteLine(new string('-', 72));
    }
}
catch (FileNotFoundException ex)
{
    Console.Error.WriteLine(ex.Message);
    Environment.ExitCode = 1;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Unexpected error: {ex.Message}");
    Environment.ExitCode = 1;
}

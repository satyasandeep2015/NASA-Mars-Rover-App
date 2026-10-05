using NasaMarsRoverConsoleApp.Models;

namespace NasaMarsRoverConsoleApp.Services;

public sealed class RoverPhotoProcessor
{
    private readonly DateParser _dateParser;
    private readonly NasaMarsRoverClient _nasaClient;
    private readonly PhotoDownloader _downloader;

    public RoverPhotoProcessor(DateParser dateParser, NasaMarsRoverClient nasaClient, PhotoDownloader downloader)
    {
        _dateParser = dateParser;
        _nasaClient = nasaClient;
        _downloader = downloader;
    }

    public async Task<IReadOnlyList<DateProcessingResult>> ProcessFileAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Input dates file was not found.", filePath);

        var results = new List<DateProcessingResult>();
        var lines = await File.ReadAllLinesAsync(filePath, cancellationToken);

        foreach (var line in lines)
        {
            var parseResult = _dateParser.Parse(line);
            var result = new DateProcessingResult
            {
                Input = line,
                Date = parseResult.Date
            };

            if (!parseResult.IsValid)
            {
                result.Errors.Add(parseResult.Error ?? "Invalid date.");
                results.Add(result);
                continue;
            }

            try
            {
                var photos = await _nasaClient.GetPhotosAsync(parseResult.Date!.Value, 5, cancellationToken);
                result.PhotosFound = photos.Count;

                foreach (var photo in photos)
                {
                    try
                    {
                        var downloaded = await _downloader.DownloadAsync(photo, parseResult.Date.Value, cancellationToken);
                        if (downloaded)
                            result.Downloaded++;
                        else
                            result.Skipped++;
                    }
                    catch (Exception ex) when (ex is HttpRequestException or IOException)
                    {
                        result.Errors.Add($"Photo {photo.Id}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                result.Errors.Add($"NASA API error: {ex.Message}");
            }

            results.Add(result);
        }

        return results;
    }
}

using NasaMarsRoverConsoleApp.Models;

namespace NasaMarsRoverConsoleApp.Services;

public sealed class RoverPhotoProcessor
{
    private readonly DateParser _dateParser;
    private readonly NasaMarsRoverClient _nasaClient;
    private readonly PhotoDownloader _downloader;

    public RoverPhotoProcessor(
        DateParser dateParser,
        NasaMarsRoverClient nasaClient,
        PhotoDownloader downloader)
    {
        _dateParser = dateParser;
        _nasaClient = nasaClient;
        _downloader = downloader;
    }

    /// <summary>
    /// Reads all dates from the input file and processes them concurrently.
    /// Each date is independent, so Task.WhenAll allows the I/O-bound
    /// NASA API calls to execute concurrently.
    /// </summary>
    public async Task<IReadOnlyList<DateProcessingResult>> ProcessFileAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        // Fail early if the input file does not exist.
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                "Input dates file was not found.",
                filePath);
        }

        // Read all date strings asynchronously.
        var lines = await File.ReadAllLinesAsync(
            filePath,
            cancellationToken);

        // Each date can be processed independently.
        // We create one Task per date instead of processing them sequentially.
        var tasks = lines.Select(line =>
            ProcessDateAsync(line, cancellationToken));

        // Wait for all date-processing tasks to finish.
        // This is appropriate because these operations are primarily I/O-bound.
        var results = await Task.WhenAll(tasks);

        return results;
    }

    /// <summary>
    /// Validates one date, gets rover photos for that date,
    /// and downloads the returned photos concurrently.
    /// </summary>
    private async Task<DateProcessingResult> ProcessDateAsync(
        string line,
        CancellationToken cancellationToken)
    {
        // Parse and validate the input date.
        var parseResult = _dateParser.Parse(line);

        var result = new DateProcessingResult
        {
            Input = line,
            Date = parseResult.Date
        };

        // Invalid dates such as April 31 are reported
        // without stopping the rest of the application.
        if (!parseResult.IsValid)
        {
            result.Errors.Add(
                parseResult.Error ?? "Invalid date.");

            return result;
        }

        try
        {
            // Call the rover API and request up to 5 photos.
            var photos = await _nasaClient.GetPhotosAsync(
                parseResult.Date!.Value,
                5,
                cancellationToken);

            result.PhotosFound = photos.Count;

            // Every photo can be downloaded independently.
            // Start all download operations before awaiting them.
            var downloadTasks = photos.Select(photo =>
                DownloadPhotoAsync(
                    photo,
                    parseResult.Date.Value,
                    cancellationToken));

            // Download the photos concurrently instead of one at a time.
            var downloadResults =
                await Task.WhenAll(downloadTasks);

            // Task.WhenAll has completed, so we can now safely update
            // the DateProcessingResult from a single execution flow.
            foreach (var downloadResult in downloadResults)
            {
                if (downloadResult.Error != null)
                {
                    result.Errors.Add(downloadResult.Error);
                }
                else if (downloadResult.Downloaded)
                {
                    result.Downloaded++;
                }
                else
                {
                    // DownloadAsync returns false when the file
                    // already exists, so count it as skipped.
                    result.Skipped++;
                }
            }
        }
        catch (Exception ex)
            when (ex is HttpRequestException or TaskCanceledException)
        {
            // An API/network failure for one date should not stop
            // the remaining dates from being processed.
            result.Errors.Add(
                $"NASA API error: {ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// Downloads a single photo and returns the outcome instead of
    /// modifying shared state from inside a concurrent task.
    /// </summary>
    private async Task<PhotoTaskResult> DownloadPhotoAsync(
        MarsPhoto photo,
        DateTime date,
        CancellationToken cancellationToken)
    {
        try
        {
            var downloaded = await _downloader.DownloadAsync(
                photo,
                date,
                cancellationToken);

            return new PhotoTaskResult
            {
                Downloaded = downloaded
            };
        }
        catch (Exception ex)
            when (ex is HttpRequestException or IOException)
        {
            // Return the error rather than directly adding it to
            // result.Errors. Multiple download tasks may be running
            // concurrently, and List<T> is not thread-safe.
            return new PhotoTaskResult
            {
                Error = $"Photo {photo.Id}: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Internal result used by each concurrent photo-download task.
    /// This allows Task.WhenAll to collect results without multiple
    /// tasks modifying the same DateProcessingResult.
    /// </summary>
    private sealed class PhotoTaskResult
    {
        // True = a new file was downloaded.
        // False = the photo already existed and was skipped.
        public bool Downloaded { get; init; }

        // Contains an error message when the download fails.
        public string? Error { get; init; }
    }
}
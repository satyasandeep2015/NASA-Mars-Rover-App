using NasaMarsRoverConsoleApp.Models;

namespace NasaMarsRoverConsoleApp.Services;

public sealed class PhotoDownloader
{
    private readonly HttpClient _httpClient;
    private readonly string _rootDirectory;

    public PhotoDownloader(HttpClient httpClient, string rootDirectory)
    {
        _httpClient = httpClient;
        _rootDirectory = rootDirectory;
    }

    public async Task<bool> DownloadAsync(
        MarsPhoto photo,
        DateTime date,
        CancellationToken cancellationToken = default)
    {
        var dateFolder = Path.Combine(_rootDirectory, date.ToString("yyyy-MM-dd"));
        Directory.CreateDirectory(dateFolder);

        var extension = Path.GetExtension(new Uri(photo.ImageUrl).AbsolutePath);
        if (string.IsNullOrWhiteSpace(extension))
            extension = ".jpg";

        var filePath = Path.Combine(dateFolder, $"{photo.Id}{extension}");

        if (File.Exists(filePath))
            return false;

        using var response = await _httpClient.GetAsync(photo.ImageUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var fileStream = File.Create(filePath);
        await response.Content.CopyToAsync(fileStream, cancellationToken);
        return true;
    }
}

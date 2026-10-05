using System.Net.Http.Json;
using NasaMarsRoverConsoleApp.Models;

namespace NasaMarsRoverConsoleApp.Services;

public sealed class NasaMarsRoverClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public NasaMarsRoverClient(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public async Task<IReadOnlyList<MarsPhoto>> GetPhotosAsync(
        DateTime earthDate,
        int take,
        CancellationToken cancellationToken = default)
    {
        var date = earthDate.ToString("yyyy-MM-dd");
        var url = $"mars-photos/api/v1/rovers/curiosity/photos?earth_date={date}&api_key={Uri.EscapeDataString(_apiKey)}";

        using var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<MarsPhotosResponse>(cancellationToken: cancellationToken)
                      ?? new MarsPhotosResponse();

        return payload.Photos.Take(take).ToList();
    }
}

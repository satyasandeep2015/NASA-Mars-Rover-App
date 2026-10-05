using System.Net.Http.Json;
using NasaMarsRoverConsoleApp.Models;

namespace NasaMarsRoverConsoleApp.Services;

public sealed class NasaMarsRoverClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public NasaMarsRoverClient(HttpClient httpClient, string apiKey, string baseUrl)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
        _baseUrl = baseUrl;
    }

    public async Task<IReadOnlyList<MarsPhoto>> GetPhotosAsync(
        DateTime earthDate,
        int take,
        CancellationToken cancellationToken = default)
    {
        var url =$"{_baseUrl}/rovers/curiosity/photos" +$"?earth_date={earthDate:yyyy-MM-dd}";
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<MarsPhotosResponse>(cancellationToken: cancellationToken)
                      ?? new MarsPhotosResponse();

        return payload.Photos.Take(take).ToList();
    }
}

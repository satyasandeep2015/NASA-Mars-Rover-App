using System.Text.Json.Serialization;

namespace NasaMarsRoverConsoleApp.Models;

public sealed class MarsPhoto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("img_src")]
    public string ImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("earth_date")]
    public string EarthDate { get; set; } = string.Empty;
}

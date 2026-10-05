using System.Text.Json.Serialization;

namespace NasaMarsRoverConsoleApp.Models;

public sealed class MarsPhotosResponse
{
    [JsonPropertyName("photos")]
    public List<MarsPhoto> Photos { get; set; } = new();
}

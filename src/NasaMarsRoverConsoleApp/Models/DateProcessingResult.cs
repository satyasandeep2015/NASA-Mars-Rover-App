namespace NasaMarsRoverConsoleApp.Models;

public sealed class DateProcessingResult
{
    public string Input { get; init; } = string.Empty;
    public DateTime? Date { get; init; }
    public int PhotosFound { get; set; }
    public int Downloaded { get; set; }
    public int Skipped { get; set; }
    public List<string> Errors { get; } = new();
}

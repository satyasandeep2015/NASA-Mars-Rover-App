namespace NasaMarsRoverConsoleApp.Models;

public sealed record DateParseResult(string Input, DateTime? Date, string? Error)
{
    public bool IsValid => Date.HasValue;
}

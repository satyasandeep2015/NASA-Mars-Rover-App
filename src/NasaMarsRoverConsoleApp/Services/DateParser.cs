using System.Globalization;
using NasaMarsRoverConsoleApp.Models;

namespace NasaMarsRoverConsoleApp.Services;

public sealed class DateParser
{
    private static readonly string[] Formats =
    {
        "MM/dd/yy",
        "MMMM d, yyyy",
        "MMM-d-yyyy"
    };

    public DateParseResult Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return new DateParseResult(input, null, "Date value is empty.");

        if (DateTime.TryParseExact(
                input.Trim(),
                Formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date))
        {
            return new DateParseResult(input, date.Date, null);
        }

        return new DateParseResult(input, null, $"Invalid or unsupported date: '{input}'.");
    }
}

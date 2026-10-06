using NasaMarsRoverConsoleApp.Services;
using Xunit;
namespace NasaMarsRoverConsoleApp.Tests;

public class DateParserTests
{
    private readonly DateParser _parser = new();

    [Theory]
    [InlineData("02/27/17", 2017, 2, 27)]
    [InlineData("June 2, 2018", 2018, 6, 2)]
    [InlineData("Jul-13-2016", 2016, 7, 13)]
    public void Parse_ValidDates_ReturnsExpectedDate(string input, int year, int month, int day)
    {
        var result = _parser.Parse(input);

        Assert.True(result.IsValid);
        Assert.Equal(new DateTime(year, month, day), result.Date);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Parse_InvalidCalendarDate_ReturnsError()
    {
        var result = _parser.Parse("April 31, 2018");

        Assert.False(result.IsValid);
        Assert.Null(result.Date);
        Assert.NotNull(result.Error);
    }
}

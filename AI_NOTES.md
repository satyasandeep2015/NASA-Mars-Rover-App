# AI Usage Notes

## AI tools used

- ChatGPT
- GitHub Copilot can also be used during local refinement if desired.

## Prompts that worked well

1. "Design a small .NET 7 console application that reads multiple date formats from a file, validates them, calls the NASA Mars Rover Photos API, downloads up to five photos per valid date, and keeps responsibilities separated into testable classes."

2. "Implement a date parser for these formats: MM/dd/yy, MMMM d, yyyy, and MMM-d-yyyy. Invalid calendar dates such as April 31, 2018 must return a validation error instead of throwing."

3. "Review the photo download flow for duplicate downloads, async/await usage, cancellation support, and graceful handling of API/network failures."

## Example of incorrect or incomplete AI output

An early AI-generated approach used `DateTime.Parse()` for every input line. That depended on the machine's current culture and could interpret dates differently or throw for invalid values. I replaced it with `DateTime.TryParseExact()` and an explicit list of accepted formats using `CultureInfo.InvariantCulture`.

## Significant changes made after AI generation

- Kept the solution intentionally small instead of applying a multi-project Clean Architecture structure.
- Split date parsing, NASA API access, downloading, and orchestration into separate classes for readability and testing.
- Added explicit duplicate-file detection before downloading.
- Added per-photo error handling so one failed download does not stop the entire run.
- Added an environment-variable-based API key instead of hardcoding credentials.

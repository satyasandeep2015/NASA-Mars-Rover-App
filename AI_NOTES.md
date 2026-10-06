# AI Usage Notes

## AI tools used

- ChatGPT

## Prompts that worked well

1. "Design a small .NET 7 console application that reads multiple date formats from a file, validates them, calls the NASA Mars Rover Photos API, downloads up to five photos per valid date, and keeps responsibilities separated into testable classes."

2. "Implement a date parser for these formats: MM/dd/yy, MMMM d, yyyy, and MMM-d-yyyy. Invalid calendar dates such as April 31, 2018 must return a validation error instead of throwing."

3. "Review the photo download flow for duplicate downloads, async/await usage, cancellation support, graceful handling of API/network failures, and whether independent API calls and downloads can be processed concurrently."

## Example of incorrect or incomplete AI output

An early AI-generated approach used `DateTime.Parse()` for every input line. That depended on the machine's current culture and could interpret dates differently or throw for invalid values.

I replaced it with `DateTime.TryParseExact()` and an explicit list of accepted formats using `CultureInfo.InvariantCulture`.

Another improvement came from reviewing the original sequential processing flow. Since each date and each photo download are independent I/O-bound operations, I changed the implementation to use `Task.WhenAll()` so multiple requests/downloads can run concurrently.

I also avoided updating a shared `List<string>` from multiple tasks at the same time. Instead, each download task returns its own result and error information, and those results are combined after `Task.WhenAll()` completes.

## Significant changes made after AI generation

- Kept the solution intentionally small instead of applying a multi-project Clean Architecture structure.
- Split date parsing, NASA API access, downloading, and orchestration into separate classes for readability and testing.
- Added explicit duplicate-file detection before downloading.
- Added per-photo error handling so one failed download does not stop the entire run.
- Added configuration-based API settings instead of hardcoding credentials directly in the source code.
- Changed sequential processing to concurrent processing using `Task.WhenAll()` because the API calls and photo downloads are independent and I/O-bound.
- Avoided unsafe concurrent writes to shared collections by having each task return its own result.
- Considered bounded concurrency for production use so the application would not overwhelm an external API or exceed rate limits when processing a large number of dates.
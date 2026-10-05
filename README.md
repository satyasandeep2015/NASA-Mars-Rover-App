# NasaMarsRoverConsoleApp

A .NET 7 console application for the NASA Mars Rover Photos coding exercise.

## Features

- Reads dates from `dates.txt`
- Supports the required input formats
- Rejects invalid dates without crashing
- Calls the NASA Mars Rover Photos API using `earth_date`
- Fetches up to 5 Curiosity photos per valid date
- Downloads photos asynchronously
- Stores photos under `photos/yyyy-MM-dd/`
- Skips files that already exist
- Handles API and download failures gracefully
- Prints a clear console summary
- Includes xUnit tests for date parsing

## Requirements

- .NET 7 SDK
- A NASA API key is recommended

## Configure API key

Do not hardcode the key in source code.

macOS/Linux:

```bash
export NASA_API_KEY="your_key_here"
```

If `NASA_API_KEY` is not set, the app uses NASA's `DEMO_KEY`, which is rate-limited.

## Run

From the repository root:

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/NasaMarsRoverConsoleApp
```

You can optionally provide a different dates file:

```bash
dotnet run --project src/NasaMarsRoverConsoleApp -- /path/to/dates.txt
```

## Input

`dates.txt` contains:

```text
02/27/17
June 2, 2018
Jul-13-2016
April 31, 2018
```

## Output folders

Downloaded images are stored as:

```text
photos/2017-02-27/
photos/2018-06-02/
photos/2016-07-13/
```

## Assumptions

- Curiosity is used for all requests.
- The application downloads at most 5 photos per valid date.
- Existing files are considered already downloaded and are skipped.
- Individual image download failures do not stop processing of the remaining images or dates.

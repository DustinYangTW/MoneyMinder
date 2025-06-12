# MoneyMinder

MoneyMinder is a sample ASP.NET Core web application that demonstrates how to use a custom file-based logger and a simple queue library. The solution contains three projects that can be built and run together.

## Projects

- **MoneyMinder** – The main ASP.NET Core MVC application.
- **DailyFileLogger** – A logging provider that writes messages to files in `wwwroot/Log` with one log file per day.
- **QueueLibrary** – Provides an asynchronous queue (`OperationQueue`) so work items are executed sequentially.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download) or later.

### Building the Solution

Run the following command from the repository root to restore packages and build all projects:

```bash
dotnet build MoneyMinder.sln
```

### Running the Web Application

Start the ASP.NET Core application using the `dotnet run` command:

```bash
dotnet run --project MoneyMinder
```

The application will listen on the URLs configured in `Properties/launchSettings.json` (by default `https://localhost:7176` and `http://localhost:5257`).

## Logging

`DailyFileLogger` writes log messages to `wwwroot/Log/yyyy-MM/yyyy-MM-dd.log`. The logger is registered in `Program.cs` using `builder.Logging.AddDailyFileLogger()` and is configured alongside console and debug logging. Sample log entries look like the following:

```
2024-03-25 10:15:30.123 [Information] MoneyMinder.Controllers.HomeController: Index action executed
```

## Using the Queue Library

`OperationQueue` ensures that asynchronous tasks are executed one at a time. `DailyFileLogger` uses this queue so that file writes are performed sequentially, but the queue can also be used by other components when sequential processing is required.

## Directory Layout

```
MoneyMinder.sln              Solution file
MoneyMinder/                 ASP.NET Core MVC web app
DailyFileLogger/             Custom logging provider
QueueLibrary/                Async queue implementation
```

## License

This repository does not currently contain a license file. If you plan to use the code beyond learning or personal experiments, please add an appropriate license.


# Logging

When building or maintaining an identity server, robust logging is essential for diagnosing issues in production and understanding application behavior.
By default, ASP.NET Core includes a built-in logging framework provided by Microsoft. For more details, see the official documentation: [https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-9.0](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-9.0).

However, we recommend using the [Serilog.AspNetCore](https://github.com/serilog/serilog-aspnetcore) library for several compelling reasons

1. **Contextual Enrichment** : Serilog can automatically enrich every log event with contextual properties—such as the machine name, the current user’s name, or the request identifier—without any extra work in your code. This additional metadata makes troubleshooting in production much simpler, since you immediately know where and why a given log entry was recorded.

2. **Flexible and Powerful Sinks** : Serilog offers dozens of “sink” packages that let you route logs to a variety of targets. A few popular examples include:
   * **Serilog.Sinks.Seq**: sends log events to the [Seq dashboard]((https://datalust.co/seq)) for interactive querying.
   * **Serilog.Sinks.Elasticsearch** : pushes logs into an Elasticsearch cluster, where they can be indexed and searched at scale.

To get started, add the Serilog.AspNetCore NuGet package to your identity server project:

```bash  title="cmd.exe"
dotnet add package Serilog.AspNetCore
```

Next, open your Program.cs file and replace the default logging configuration with Serilog. The following example demonstrates how to:

* Define minimum log levels globally and for specific namespaces
* Write logs both to the console (with a colored, human-readable template) and to daily rolling files

[Demo](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/IdServerLogging).

```csharp title="Program.cs"
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
    .CreateLogger();
webApplicationBuilder.Logging.AddSerilog();
```
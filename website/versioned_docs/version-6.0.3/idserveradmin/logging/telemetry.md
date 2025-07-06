# Open telemetry

Assuming OpenTelemetry has been enabled on your identity server by following this tutorial, the administration site can be configured to propagate OpenTelemetry tracing information to the [identity server](../../idserver/logging/telemetry.md).

To do this, install the NuGet package `SimpleIdServer.IdServer.Website.OpenTelemetry` and edit the `Program.cs` file.

```bash  title="cmd.exe"
dotnet add package SimpleIdServer.IdServer.Website.OpenTelemetry
```

Call the `EnableOpenTelemetry` function, which accepts a callback to configure tracing.

For example:

```csharp title="Program.cs"
EnableOpenTelemetry(c =>
{
    c.AddConsoleExporter();
    c.AddOtlpExporter(o =>
    {
        o.Endpoint = new Uri("");
        o.Headers = "";
        o.Protocol = OtlpExportProtocol.HttpProtobuf;
    });
});
```
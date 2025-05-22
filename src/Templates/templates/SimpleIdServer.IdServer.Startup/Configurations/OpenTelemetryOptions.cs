// Copyright (c) SimpleIdServer. AllClients rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using OpenTelemetry.Exporter;

namespace SimpleIdServer.IdServer.Startup.Configurations;

public class OpenTelemetryOptions
{
    public bool EnableOtpExported
    {
        get; set;
    }

    public bool EnableConsoleExporter 
    { 
        get; set;
    } = true;

    public bool EnableEfCoreTracing
    {
        get; set;
    } = true;

    public string MetricsEndpoint
    { 
        get; set;
    }

    public string TracesEndpoint
    {
        get; set;
    }

    public string Headers
    {
        get; set;
    }

    public OtlpExportProtocol Protocol
    {
        get; set;
    } = OtlpExportProtocol.HttpProtobuf;
}

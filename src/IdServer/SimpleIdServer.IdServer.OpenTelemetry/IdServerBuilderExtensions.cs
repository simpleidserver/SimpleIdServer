// Copyright (c) SimpleIdServer. AllClients rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SimpleIdServer.IdServer;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerBuilderExtensions
{
    public static IdServerBuilder EnableOpenTelemetry(this IdServerBuilder builder, Action<MeterProviderBuilder> metricsCb = null, Action<TracerProviderBuilder> tracingCb = null)
    {
        builder.Services.AddOpenTelemetry()
            .WithMetrics(m =>
            {
                m.AddMeter(Counters.ServiceName);
                if (metricsCb != null)
                {
                    metricsCb(m);
                }
            })
            .WithTracing(t =>
            {
                t.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Builder.Environment.ApplicationName))
                .AddSource(Tracing.Names.Basic)
                .AddSource(Tracing.Names.Cache)
                .AddSource(Tracing.Names.Store)
                .AddAspNetCoreInstrumentation(o =>
                {
                    o.EnrichWithHttpRequest = (activity, request) =>
                    {
                        activity.DisplayName = $"{request.Method.ToString()} {request.Path}";
                    };
                    o.Filter = (httpContext) =>
                    {
                        var path = httpContext.Request.Path.ToString();
                        if (path.StartsWith("/_blazor") || new string[] { ".css", ".js", ".png", ".svg" }.Any(s => path.EndsWith(s)))
                        {
                            return false;
                        }

                        return true;
                    };
                })
                .AddHttpClientInstrumentation();
                if(tracingCb != null)
                {
                    tracingCb(t);
                }
            });
        return builder;
    }
}

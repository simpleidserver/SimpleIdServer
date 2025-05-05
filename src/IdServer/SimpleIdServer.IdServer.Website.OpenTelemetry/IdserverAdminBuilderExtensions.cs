// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SimpleIdServer.IdServer.Website;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdserverAdminBuilderExtensions
{
    public static IdserverAdminBuilder EnableOpenTelemetry(this IdserverAdminBuilder builder, Action<TracerProviderBuilder> tracingCb = null)
    {
        builder.Builder.Services.AddOpenTelemetry()
            .WithTracing(t =>
            {
                t.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Builder.Environment.ApplicationName))
                .AddAspNetCoreInstrumentation(o =>
                {
                    o.EnrichWithHttpRequest = (activity, request) =>
                    {
                        activity.DisplayName = $"{request.Method.ToString()} {request.Path}";
                    };
                    o.EnrichWithHttpResponse = (activity, resp) =>
                    {
                        var endpoint = resp.HttpContext.GetEndpoint() as RouteEndpoint;
                        if (endpoint is not null)
                        {
                            var urlPath = activity.Tags.FirstOrDefault(t => t.Key == "url.path");
                            if (urlPath.Value != null)
                            {
                                activity.DisplayName = $"{resp.HttpContext.Request.Method} {urlPath.Value}";
                            }
                        }
                    };
                    o.Filter = (httpContext) =>
                    {
                        var path = httpContext.Request.Path.ToString();
                        if (path.StartsWith("/_blazor") || new string[] { ".css", ".js", ".png" }.Any(s => path.EndsWith(s)))
                        {
                            return false;
                        }

                        return true;
                    };
                })
                .AddHttpClientInstrumentation(o =>
                {
                    o.EnrichWithHttpRequestMessage = (activity, request) =>
                    {
                        var url = string.Empty;
                        if (request.RequestUri != null)
                        {
                            url = request.RequestUri.AbsolutePath;
                        }

                        activity.DisplayName = $"{request.Method.ToString()} {url}";
                    };
                });
                if(tracingCb != null)
                {
                    tracingCb(t);
                }
            });
        return builder;
    }
}

// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Helpers;
using Microsoft.AspNetCore.Http;

namespace FormBuilder.Middlewares;

public class HttpRequestStateMiddleware
{
    private readonly RequestDelegate _next;

    public HttpRequestStateMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IHttpRequestState httpRequestState)
    {
        if (!httpRequestState.IsInitialized)
        {
            var request = context.Request;
            var scheme = request.IsHttps ? "https" : "http";
            httpRequestState.Initialize(scheme, request.Host.Value, request.PathBase.Value);
        }

        await _next(context);
    }
}

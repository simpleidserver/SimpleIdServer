﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;

namespace SimpleIdServer.IdServer.Website.Middlewares;

public class HttpsMiddleware
{
    private readonly RequestDelegate _next;

    public HttpsMiddleware(RequestDelegate next)
    {

        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Request.Scheme = "https";
        await _next(context);
    }
}

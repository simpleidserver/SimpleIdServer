// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.CredentialIssuer.Middlewares;

public class MissingContentTypeMiddleware
{
    private readonly RequestDelegate _next;

    public MissingContentTypeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var methods = new[] { "POST", "PUT" };
        if(methods.Contains(context.Request.Method) && string.IsNullOrWhiteSpace(context.Request.Headers.ContentType))
        {
            context.Request.Headers.ContentType = "application/json";
        }

        await _next(context);
    }
}

// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Middlewares;

public class LanguageMiddleware
{
    private readonly RequestDelegate _requestDelegate;

    public LanguageMiddleware(RequestDelegate requestDelegate)
    {
        _requestDelegate = requestDelegate;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if(context.Request.Headers.ContainsKey("Language"))
        {
            var language = context.Request.Headers["Language"].First();
            CultureInfo.CurrentCulture = new CultureInfo(language);
            CultureInfo.CurrentUICulture = new CultureInfo(language);
        }

        await _requestDelegate.Invoke(context);
    }
}

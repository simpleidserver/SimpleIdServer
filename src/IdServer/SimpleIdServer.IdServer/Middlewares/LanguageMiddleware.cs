// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Middlewares;

public class LanguageMiddleware
{
    private readonly RequestDelegate _requestDelegate;
    private readonly ILogger<LanguageMiddleware> _logger;

    public LanguageMiddleware(
        RequestDelegate requestDelegate,
        ILogger<LanguageMiddleware> logger)
    {
        _requestDelegate = requestDelegate;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.ContainsKey("Language"))
        {
            var language = context.Request.Headers["Language"].First();
            CultureInfo.CurrentCulture = new CultureInfo(language);
            CultureInfo.CurrentUICulture = new CultureInfo(language);
        }

        await _requestDelegate.Invoke(context);
    }
}

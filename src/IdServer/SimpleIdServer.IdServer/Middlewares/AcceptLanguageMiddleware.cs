// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Middlewares;

public class AcceptLanguageMiddleware
{
    private readonly RequestDelegate _requestDelegate;

    public AcceptLanguageMiddleware(RequestDelegate requestDelegate)
    {
        _requestDelegate = requestDelegate;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var acceptLanguage = context.Request.Headers.AcceptLanguage.ToString();
        var defaultLanguage = Domains.Language.Default;
        if(!string.IsNullOrWhiteSpace(acceptLanguage))
        {
            // Take only the first language.
            var splitted = acceptLanguage.Split(',');
            defaultLanguage = splitted.First();
        }

        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(defaultLanguage);
        await _requestDelegate.Invoke(context);
    }
}

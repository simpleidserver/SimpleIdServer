// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace SimpleIdServer.IdServer.Website;

public class CookieConfigureNamedOptions : IConfigureNamedOptions<CookieAuthenticationOptions>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Action<CookieAuthenticationOptions, HttpContext> _configureAction;

    public CookieConfigureNamedOptions(IHttpContextAccessor httpContextAccessor, Action<CookieAuthenticationOptions, HttpContext> configureAction)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _configureAction = configureAction ?? throw new ArgumentNullException(nameof(configureAction));
    }

    public void Configure(string name, CookieAuthenticationOptions options)
    {
        if (!string.Equals(name, CookieAuthenticationDefaults.AuthenticationScheme, StringComparison.Ordinal))
        {
            return;
        }

        if (_httpContextAccessor?.HttpContext == null)
        {
            throw new InvalidOperationException("HttpContext is not available.");
        }

        _configureAction(options, _httpContextAccessor.HttpContext);
    }

    public void Configure(CookieAuthenticationOptions options)
        => Configure(string.Empty, options);
}

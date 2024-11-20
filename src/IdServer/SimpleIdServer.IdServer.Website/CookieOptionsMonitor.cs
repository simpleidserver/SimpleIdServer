// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace SimpleIdServer.IdServer.Website;

public class CookieOptionsMonitor : IOptionsMonitor<CookieAuthenticationOptions>
{
    private readonly IOptionsFactory<CookieAuthenticationOptions> _optionsFactory;

    public CookieOptionsMonitor(IOptionsFactory<CookieAuthenticationOptions> optionsFactory)
    {
        _optionsFactory = optionsFactory;
    }

    public CookieAuthenticationOptions CurrentValue => Get(string.Empty);

    public CookieAuthenticationOptions Get(string name)
    {
        return _optionsFactory.Create(name);
    }

    public IDisposable OnChange(Action<CookieAuthenticationOptions, string> listener) => null;
}

// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Helpers;

namespace SimpleIdServer.IdServer.Website.Infrastructures;

public class CookieRealmStore : IRealmStore
{
    public const string DefaultRealmCookieName = "SidWebsiteCurrentRealm";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IdServerWebsiteOptions _configuration;
    private string _realm;

    public CookieRealmStore(IHttpContextAccessor httpContextAccessor, IOptions<IdServerWebsiteOptions> configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration.Value;
    }

    public string Realm
    {
        get
        {
            if(_configuration.IsReamEnabled)
            {
                return Constants.DefaultRealm;
            }

            if (!string.IsNullOrWhiteSpace(_realm))
            {
                return _realm;
            }

            var realmCookie = _httpContextAccessor.HttpContext?.Request?.Cookies?.FirstOrDefault(c => c.Key == DefaultRealmCookieName);
            return realmCookie?.Value ?? string.Empty;
        }
        set
        {
            _realm = value;
        }
    }
}
// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using SimpleIdServer.IdServer.Helpers;
using System.Linq;

namespace SimpleIdServer.IdServer.Infastructures;

public class CookieRealmStore : IRealmStore
{
    public const string DefaultRealmCookieName = "IdServerRealm";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string _realm;

    public CookieRealmStore(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string Realm
    {
        get
        {
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
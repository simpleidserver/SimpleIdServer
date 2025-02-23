// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using System.Linq;

namespace SimpleIdServer.IdServer.Infastructures;

public class CookieRealmStore : IRealmStore
{
    public const string DefaultRealmCookieName = "IdServerRealm";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IdServerHostOptions _idServerHostOptions;
    private string _realm;

    public CookieRealmStore(IHttpContextAccessor httpContextAccessor, IOptions<IdServerHostOptions> idServerHostOptions)
    {
        _httpContextAccessor = httpContextAccessor;
        _idServerHostOptions = idServerHostOptions.Value;
    }

    public string Realm
    {
        get
        {
            if (!_idServerHostOptions.UseRealm)
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
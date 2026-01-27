// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Helpers;

namespace SimpleIdServer.IdServer.Website.Helpers;

public interface IUrlHelper
{
    string GetUrl(string url);
}

public class UrlHelper : IUrlHelper
{
    private readonly IdServerWebsiteOptions _options;
    private readonly IRealmStore _realmStore;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UrlHelper(
        IOptions<IdServerWebsiteOptions> options, 
        IRealmStore realmStore, 
        IHttpContextAccessor httpContextAccessor)
    {
        _options = options.Value;
        _realmStore = realmStore;
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetUrl(string url)
    {
        var pathBase = _httpContextAccessor.HttpContext?.Request.PathBase.Value?.TrimEnd('/') ?? string.Empty;        
        if (!_options.IsReamEnabled)
        {
            return string.IsNullOrEmpty(pathBase) ? url : $"{pathBase}{url}";
        }
        
        var realmPath = string.IsNullOrWhiteSpace(_realmStore.Realm) ? string.Empty : $"/{_realmStore.Realm}";
        return $"{pathBase}{realmPath}{url}";
    }
}
// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using SimpleIdServer.IdServer.UI.ViewModels;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Helpers;

public interface IAcrHelper
{
    Task StoreAcr(AcrAuthInfo acrAuthInfo, CancellationToken cancellationToken);
    Task<AcrAuthInfo> GetAcr(CancellationToken cancellationToken);
}

public class AcrHelper : IAcrHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDistributedCache _distributedCache;

    public AcrHelper(IHttpContextAccessor httpContextAccessor, IDistributedCache distributedCache)
    {
        _httpContextAccessor = httpContextAccessor;
        _distributedCache = distributedCache;
    }

    public async Task StoreAcr(AcrAuthInfo acrAuthInfo, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid().ToString();
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext.Request.Cookies.ContainsKey(Constants.DefaultCurrentAcrCookieName))
            id = httpContext.Request.Cookies[Constants.DefaultCurrentAcrCookieName];
        await _distributedCache.SetStringAsync(id, JsonSerializer.Serialize(acrAuthInfo), cancellationToken);
        httpContext.Response.Cookies.Append(Constants.DefaultCurrentAcrCookieName, id);
    }

    public async Task<AcrAuthInfo> GetAcr(CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (!httpContext.Request.Cookies.ContainsKey(Constants.DefaultCurrentAcrCookieName)) return null;
        var id = httpContext.Request.Cookies[Constants.DefaultCurrentAcrCookieName];
        var serializedData = await _distributedCache.GetStringAsync(id, cancellationToken);
        if (string.IsNullOrWhiteSpace(serializedData)) return null;
        return JsonSerializer.Deserialize<AcrAuthInfo>(serializedData);
    }
}

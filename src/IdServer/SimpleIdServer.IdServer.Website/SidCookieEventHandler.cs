// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;
using System.IdentityModel.Tokens.Jwt;

namespace SimpleIdServer.IdServer.Website;

public class SidCookieEventHandler : CookieAuthenticationEvents
{
    private readonly IDistributedCache _distributedCache;

    public SidCookieEventHandler(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        if (context.Principal.Identity.IsAuthenticated)
        {
            var subject = context.Principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var sessionId = context.Principal.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;
            var cacheKey = $"{subject}_{sessionId}";
            var cacheValue = await _distributedCache.GetStringAsync(cacheKey);
            if (!string.IsNullOrWhiteSpace(cacheValue))
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await _distributedCache.RemoveAsync(cacheKey);
            }
        }
    }
}

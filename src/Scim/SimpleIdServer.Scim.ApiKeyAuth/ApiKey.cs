// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using AspNetCore.Authentication.ApiKey;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace SimpleIdServer.Scim.ApiKeyAuth;

public class ApiKey : IApiKey
{
    public ApiKey(string key, string owner, IEnumerable<string> scopes)
    {
        Key = key;
        OwnerName = owner;
        var claims = scopes.Select(s => new Claim("scope", s)).ToList();
        claims.Add(new Claim(ClaimTypes.NameIdentifier, owner));
        Claims = claims;
    }

    public string Key { get; set; }
    public string OwnerName { get; set; }
    public IReadOnlyCollection<Claim> Claims { get; set; }
}

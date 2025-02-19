// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.JsonWebTokens;
using System.Collections.Concurrent;

namespace SimpleIdServer.IdServer.Website;

public interface IAccessTokenStore
{
    ConcurrentDictionary<string, GetAccessTokenResult> AccessTokens { get; set; }
}

public class AccessTokenStore : IAccessTokenStore
{
    public ConcurrentDictionary<string, GetAccessTokenResult> AccessTokens
    {
        get; set;
    } = new ConcurrentDictionary<string, GetAccessTokenResult>();
}

public record GetAccessTokenResult
{
    public GetAccessTokenResult(string accessToken, JsonWebToken jwt)
    {
        AccessToken = accessToken;
        Jwt = jwt;
    }

    public string AccessToken { get; private set; }
    public JsonWebToken Jwt { get; private set; }
    public bool IsValid => Jwt != null && Jwt.ValidTo >= DateTime.UtcNow;
}
// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;

namespace SimpleIdServer.FastFed.ApplicationProvider.Resolvers;

public interface IIssuerResolver
{
    string Get();
}

public class IssuerResolver : IIssuerResolver
{
    private readonly IHttpContextAccessor _accessor;

    public IssuerResolver(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public string Get()
    {
        var requestMessage = _accessor.HttpContext.Request;
        var host = requestMessage.Host.Value;
        var http = "http://";
        if (requestMessage.IsHttps) http = "https://";
        var relativePath = requestMessage.PathBase.Value;
        return http + host + relativePath;
    }
}

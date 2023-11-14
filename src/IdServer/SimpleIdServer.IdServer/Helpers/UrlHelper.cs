// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Options;

namespace SimpleIdServer.IdServer.Helpers;

public interface IUrlHelper
{
    string GetAbsoluteUriWithVirtualPath(HttpRequest request);
}

public class UrlHelper : IUrlHelper
{
    private readonly IdServerHostOptions _options;

    public UrlHelper(IOptions<IdServerHostOptions> options)
    {
        _options = options.Value;
    }

    public string GetAbsoluteUriWithVirtualPath(HttpRequest request)
    {
        if(!string.IsNullOrWhiteSpace(_options.BaseUrl)) return _options.BaseUrl;
        return request.GetAbsoluteUriWithVirtualPath();
    }
}

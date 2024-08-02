// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Options;

namespace SimpleIdServer.IdServer.Website.Helpers;

public interface IUrlHelper
{
    string GetUrl(string url);
}

public class UrlHelper : IUrlHelper
{
    private readonly IdServerWebsiteOptions _options;

    public UrlHelper(IOptions<IdServerWebsiteOptions> options)
    {
        _options = options.Value;
    }

    public string GetUrl(string url)
    {
        if (!_options.IsReamEnabled) return url;
        var realmContext = IdServer.Helpers.RealmContext.Instance();
        return string.IsNullOrWhiteSpace(realmContext.Realm) ? url : $"/{realmContext.Realm}{url}";
    }
}
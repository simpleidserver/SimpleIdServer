// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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

    public UrlHelper(IOptions<IdServerWebsiteOptions> options, IRealmStore realmStore)
    {
        _options = options.Value;
        _realmStore = realmStore;
    }

    public string GetUrl(string url)
    {
        if (!_options.IsReamEnabled) return url;
        return string.IsNullOrWhiteSpace(_realmStore.Realm) ? url : $"/{_realmStore.Realm}{url}";
    }
}
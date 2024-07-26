// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Website.Infrastructures;

namespace SimpleIdServer.IdServer.Website.Helpers;

public interface IUrlHelper
{
    string GetUrl(string url);
}

public class UrlHelper : IUrlHelper
{
    private readonly CurrentRealm _currentRealm;

    public UrlHelper(CurrentRealm currentRealm)
    {
        _currentRealm = currentRealm;    
    }

    public string GetUrl(string url)
        => string.IsNullOrWhiteSpace(_currentRealm.Identifier) ? url : $"/{_currentRealm.Identifier}{url}";
}

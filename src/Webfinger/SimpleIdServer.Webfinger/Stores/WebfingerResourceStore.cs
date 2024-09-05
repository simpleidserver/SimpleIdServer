// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Webfinger.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Webfinger.Stores;

public interface IWebfingerResourceStore
{
    Task<List<WebfingerResource>> GetResources(string scheme, string subject, List<string> rels, CancellationToken cancellationToken);
}

public class WebfingerResourceStore : IWebfingerResourceStore
{
    public WebfingerResourceStore()
    {
        
    }

    public Task<List<WebfingerResource>> GetResources(string scheme, string subject, List<string> rels, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}

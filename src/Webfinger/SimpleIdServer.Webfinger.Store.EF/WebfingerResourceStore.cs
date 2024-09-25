// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Webfinger.Models;
using SimpleIdServer.Webfinger.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Webfinger.Store.EF;

public class WebfingerResourceStore : IWebfingerResourceStore
{
    private readonly WebfingerDbContext _dbContext;

    public WebfingerResourceStore(WebfingerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<WebfingerResource>> GetResources(string scheme, string subject, List<string> rels, CancellationToken cancellationToken)
    {
        var result = await _dbContext.WebfingerResources.Where(r => r.Scheme == scheme && r.Subject == subject && rels.Contains(r.Rel)).ToListAsync(cancellationToken);
        return result;
    }
}

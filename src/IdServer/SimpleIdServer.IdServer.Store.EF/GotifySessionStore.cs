// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF;

public class GotiySessionStore : IGotiySessionStore
{
    private readonly StoreDbContext _dbContext;

    public GotiySessionStore(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(GotifySession session)
        => _dbContext.GotifySessions.Add(session);

    public Task<GotifySession> GetByClientToken(string clientToken, CancellationToken cancellationToken)
        => _dbContext.GotifySessions.SingleOrDefaultAsync(c => c.ClientToken == clientToken, cancellationToken);

    public Task<int> SaveChanges(CancellationToken cancellationToken)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
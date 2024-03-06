// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store;

public interface IGotiySessionStore
{
    void Add(GotifySession session);
    Task<GotifySession> GetByClientToken(string clientToken, CancellationToken cancellationToken);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}

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
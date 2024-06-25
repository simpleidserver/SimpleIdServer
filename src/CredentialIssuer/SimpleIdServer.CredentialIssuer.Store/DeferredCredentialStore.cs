// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.CredentialIssuer.Domains;

namespace SimpleIdServer.CredentialIssuer.Store;

public interface IDeferredCredentialStore
{
    void Add(DeferredCredential deferredCredential);
    Task<DeferredCredential> Get(string transactionId, CancellationToken cancellationToken);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}

public class DeferredCredentialStore : IDeferredCredentialStore
{
    private readonly CredentialIssuerDbContext _dbContext;

    public DeferredCredentialStore(CredentialIssuerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(DeferredCredential deferredCredential)
        => _dbContext.DeferredCredentials.Add(deferredCredential);

    public Task<DeferredCredential> Get(string transactionId, CancellationToken cancellationToken)
        => _dbContext.DeferredCredentials.Include(c => c.Claims).SingleOrDefaultAsync(d => d.TransactionId == transactionId, cancellationToken);

    public Task<int> SaveChanges(CancellationToken cancellationToken)
        => _dbContext.SaveChangesAsync(cancellationToken);
}

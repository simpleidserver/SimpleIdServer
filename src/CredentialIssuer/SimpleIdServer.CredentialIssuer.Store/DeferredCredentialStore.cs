// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.CredentialIssuer.Domains;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.CredentialIssuer.Store;

public interface IDeferredCredentialStore
{
    void Add(DeferredCredential deferredCredential);
    void Delete(DeferredCredential deferredCredential);
    Task<SearchResult<DeferredCredential>> Search(SearchRequest request, CancellationToken cancellationToken);
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

    public void Delete(DeferredCredential deferredCredential)
        => _dbContext.DeferredCredentials.Remove(deferredCredential);

    public async Task<SearchResult<DeferredCredential>> Search(SearchRequest request, CancellationToken cancellationToken)
    {
        var query = _dbContext.DeferredCredentials
            .Include(d => d.Configuration)
            .AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);
        else
            query = query.OrderByDescending(r => r.CreateDateTime);
        var nb = query.Count();
        var deferredCredentials = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync();
        return new SearchResult<DeferredCredential>
        {
            Count = nb,
            Content = deferredCredentials
        };
    }

    public Task<DeferredCredential> Get(string transactionId, CancellationToken cancellationToken)
        => _dbContext.DeferredCredentials.Include(c => c.Claims).SingleOrDefaultAsync(d => d.TransactionId == transactionId, cancellationToken);

    public Task<int> SaveChanges(CancellationToken cancellationToken)
        => _dbContext.SaveChangesAsync(cancellationToken);
}

// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Store.EF;

public class CertificateAuthorityRepository : ICertificateAuthorityRepository
{
    private readonly StoreDbContext _dbContext;

    public CertificateAuthorityRepository(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<CertificateAuthority> Get(string id, CancellationToken cancellationToken)
    {
        return _dbContext.CertificateAuthorities
            .Include(r => r.ClientCertificates)
            .SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public Task<CertificateAuthority> Get(string realm, string id, CancellationToken cancellationToken)
    {
        return _dbContext.CertificateAuthorities
            .Include(r => r.ClientCertificates)
            .Include(r => r.Realms)
            .SingleOrDefaultAsync(c => c.Id == id && c.Realms.Any(r => r.Name == realm), cancellationToken);
    }

    public async Task<SearchResult<CertificateAuthority>> Search(string realm, SearchRequest request, CancellationToken cancellationToken)
    {
        IQueryable<CertificateAuthority> query = _dbContext.CertificateAuthorities
            .Include(p => p.Realms)
            .Where(p => p.Realms.Any(r => r.Name == realm))
            .AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);
        else
            query = query.OrderByDescending(c => c.UpdateDateTime);

        var nb = query.Count();
        var cas = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync(cancellationToken);
        return new SearchResult<CertificateAuthority>
        {
            Count = nb, 
            Content = cas
        };
    }

    public void Add(CertificateAuthority certificateAuthority)
        => _dbContext.CertificateAuthorities.Add(certificateAuthority);

    public void Delete(CertificateAuthority certificateAuthority)
        => _dbContext.CertificateAuthorities.Remove(certificateAuthority);

    public Task<int> SaveChanges(CancellationToken cancellationToken)
        => _dbContext.SaveChangesAsync(cancellationToken);
}

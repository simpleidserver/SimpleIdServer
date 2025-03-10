// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultCertificateAuthorityRepository : ICertificateAuthorityRepository
{
    private readonly List<CertificateAuthority> _cas;

    public DefaultCertificateAuthorityRepository(List<CertificateAuthority> cas) => _cas = cas;

    public Task<CertificateAuthority> Get(string id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_cas.SingleOrDefault(c => c.Id == id));
    }

    public Task<CertificateAuthority> Get(string realm, string id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_cas.SingleOrDefault(c => c.Id == id && c.Realms.Any(r => r.Name == realm)));
    }

    public async Task<SearchResult<CertificateAuthority>> Search(string realm, SearchRequest request, CancellationToken cancellationToken)
    {
        IQueryable<CertificateAuthority> query = _cas.AsQueryable()
            .Where(p => p.Realms.Any(r => r.Name == realm));
        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);
        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);
        else
            query = query.OrderByDescending(c => c.UpdateDateTime);
        int count = query.Count();
        var cas = query.Skip(request.Skip.Value).Take(request.Take.Value).ToList();
        return await Task.FromResult(new SearchResult<CertificateAuthority>
        {
            Count = count,
            Content = cas
        });
    }

    public void Add(CertificateAuthority certificateAuthority) => _cas.Add(certificateAuthority);

    public void Delete(CertificateAuthority certificateAuthority) => _cas.Remove(certificateAuthority);

    public void Update(CertificateAuthority certificateAuthority)
    {
        var existing = _cas.SingleOrDefault(c => c.Id == certificateAuthority.Id);
        if (existing != null)
        {
            _cas.Remove(existing);
            _cas.Add(certificateAuthority);
        }
    }
}

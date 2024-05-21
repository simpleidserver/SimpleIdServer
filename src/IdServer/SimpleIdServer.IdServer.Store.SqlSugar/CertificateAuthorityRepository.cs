// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class CertificateAuthorityRepository : ICertificateAuthorityRepository
{
    private readonly DbContext _dbContext;

    public CertificateAuthorityRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(CertificateAuthority certificateAuthority)
    {
        _dbContext.Client.InsertNav(Transform(certificateAuthority))
            .Include(c => c.Realms)
            .ExecuteCommand();
    }

    public void Delete(CertificateAuthority certificateAuthority)
    {
        _dbContext.Client.Deleteable(Transform(certificateAuthority))
            .ExecuteCommand();
    }

    public void Update(CertificateAuthority certificateAuthority)
    {
        _dbContext.Client.UpdateNav(Transform(certificateAuthority))
            .Include(c => c.Realms)
            .Include(c => c.ClientCertificates)
            .ExecuteCommand();
    }

    public async Task<CertificateAuthority> Get(string id, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarCertificateAuthority>()
            .Includes(r => r.ClientCertificates)
            .FirstAsync(c => c.Id == id, cancellationToken);
        return result?.ToDomain();
    }

    public async Task<CertificateAuthority> Get(string realm, string id, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarCertificateAuthority>()
            .Includes(r => r.ClientCertificates)
            .Includes(r => r.Realms)
            .FirstAsync(c => c.Id == id && c.Realms.Any(r => r.RealmsName == realm), cancellationToken)
        return result?.ToDomain();
    }

    public async Task<SearchResult<CertificateAuthority>> Search(string realm, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Client.Queryable<SugarCertificateAuthority>()
            .Includes(p => p.Realms)
            .Where(p => p.Realms.Any(r => r.RealmsName == realm));
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
            Content = cas.Select(c => c.ToDomain()).ToList()
        };
    }

    private SugarCertificateAuthority Transform(CertificateAuthority certificateAuthority)
    {
        return new SugarCertificateAuthority
        {
            EndDateTime = certificateAuthority.EndDateTime,
            FindType = certificateAuthority.FindType,
            FindValue = certificateAuthority.FindValue,
            Id = certificateAuthority.Id,
            PrivateKey = certificateAuthority.PrivateKey,
            PublicKey = certificateAuthority.PublicKey,
            Source = certificateAuthority.Source,
            StoreLocation = certificateAuthority.StoreLocation,
            StartDateTime = certificateAuthority.StartDateTime,
            StoreName = certificateAuthority.StoreName,
            SubjectName = certificateAuthority.SubjectName,
            UpdateDateTime = certificateAuthority.UpdateDateTime,
            ClientCertificates = certificateAuthority.ClientCertificates == null ? new List<SugarClientCertificate>() : certificateAuthority.ClientCertificates.Select(c => new SugarClientCertificate
            {
                CreateDateTime = c.CreateDateTime,
                EndDateTime = c.EndDateTime,
                Id = c.Id,
                Name = c.Name,
                PrivateKey = c.PrivateKey,
                PublicKey = c.PublicKey,
                StartDateTime = c.StartDateTime
            }).ToList(),
            Realms = certificateAuthority.Realms == null ? new List<SugarRealm>() : certificateAuthority.Realms.Select(r => new SugarRealm
            {
                RealmsName = r.Name
            }).ToList()
        };
    }
}

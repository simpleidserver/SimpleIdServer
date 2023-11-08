// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface ICertificateAuthorityRepository
    {
        IQueryable<CertificateAuthority> Query();
        void Delete(CertificateAuthority cas);
        void Add(CertificateAuthority cas);
        void Delete(IEnumerable<CertificateAuthority> cas);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class CertificateAuthorityRepository : ICertificateAuthorityRepository
    {
        private readonly StoreDbContext _dbContext;

        public CertificateAuthorityRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<CertificateAuthority> Query() => _dbContext.CertificateAuthorities;

        public void Delete(CertificateAuthority ca) => _dbContext.CertificateAuthorities.Remove(ca);

        public void Delete(IEnumerable<CertificateAuthority> cas) => _dbContext.CertificateAuthorities.RemoveRange(cas);
        public void Add(CertificateAuthority cas) => _dbContext.CertificateAuthorities.Add(cas);

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);

    }
}

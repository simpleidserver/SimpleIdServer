// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface ICertificateAuthorityRepository
    {
        IQueryable<CertificateAuthority> Query();
    }

    public class CertificateAuthorityRepository : ICertificateAuthorityRepository
    {
        private readonly StoreDbContext _dbContext;

        public CertificateAuthorityRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<CertificateAuthority> Query() => _dbContext.CertificateAuthorities;
    }
}

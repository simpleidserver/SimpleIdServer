// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface ICredentialTemplateRepository
    {
        IQueryable<CredentialTemplate> Query();
        void Delete(IEnumerable<CredentialTemplate> credentialTemplates);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class CredentialTemplateRepository : ICredentialTemplateRepository
    {
        private readonly StoreDbContext _dbContext;

        public CredentialTemplateRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IQueryable<CredentialTemplate> Query() => _dbContext.CredentialTemplates;

        public void Delete(IEnumerable<CredentialTemplate> credentialTemplates) => _dbContext.CredentialTemplates.RemoveRange(credentialTemplates);


        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}

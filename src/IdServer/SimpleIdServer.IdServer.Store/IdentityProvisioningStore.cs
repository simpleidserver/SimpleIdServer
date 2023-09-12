// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IIdentityProvisioningStore
    {
        IQueryable<IdentityProvisioning> Query();
        void DeleteRange(IEnumerable<IdentityProvisioning> identityProvisioningLst);
        void Remove(IdentityProvisioning identityProvisioning);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class IdentityProvisioningStore : IIdentityProvisioningStore
    {
        private readonly StoreDbContext _dbContext;

        public IdentityProvisioningStore(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<IdentityProvisioning> Query() => _dbContext.IdentityProvisioningLst;

        public void DeleteRange(IEnumerable<IdentityProvisioning> identityProvisioningLst) => _dbContext.IdentityProvisioningLst.RemoveRange(identityProvisioningLst);

        public void Remove(IdentityProvisioning identityProvisioning) => _dbContext.IdentityProvisioningLst.Remove(identityProvisioning);

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}

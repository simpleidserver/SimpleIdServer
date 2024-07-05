// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using LinqToDB;
using SimpleIdServer.OpenidFederation.Domains;
using SimpleIdServer.OpenidFederation.Stores;

namespace SimpleIdServer.IdServer.Store.EF;

public class FederationEntityStore : IFederationEntityStore
{
    private readonly StoreDbContext _dbContext;

    public FederationEntityStore(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<FederationEntity> GetSubordinate(string sub, string realm, CancellationToken cancellationToken)
    {
        return _dbContext.FederationEntities.FirstOrDefaultAsync(r => r.Sub == sub && r.Realm == realm && r.IsSubordinate == true, cancellationToken);
    }

    public Task<List<FederationEntity>> GetAllSubordinates(string realm, CancellationToken cancellationToken)
    {
        return _dbContext.FederationEntities.Where(f => f.Realm == realm && f.IsSubordinate == true).ToListAsync(cancellationToken);
    }

    public Task<List<FederationEntity>> GetAllAuthorities(string realm, CancellationToken cancellationToken)
    {
        return _dbContext.FederationEntities.Where(f => f.Realm == realm && f.IsSubordinate == false).ToListAsync(cancellationToken);
    }
}

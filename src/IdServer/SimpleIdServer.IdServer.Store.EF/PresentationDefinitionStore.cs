// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF;

public class PresentationDefinitionStore : IPresentationDefinitionStore
{
    private readonly StoreDbContext _dbContext;

    public PresentationDefinitionStore(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(PresentationDefinition presentationDefinition)
    {
        _dbContext.PresentationDefinitions.Add(presentationDefinition);
    }

    public Task<List<PresentationDefinition>> GetAll(string realm, CancellationToken cancellationToken)
    {
        var result = _dbContext.PresentationDefinitions
            .Include(p => p.InputDescriptors)
            .Where(p => p.RealmName == realm).ToListAsync(cancellationToken);
        return result;
    }

    public Task<PresentationDefinition> GetByPublicId(string id, string realm, CancellationToken cancellationToken)
    {
        var result = _dbContext.PresentationDefinitions
            .Include(p => p.InputDescriptors).ThenInclude(p => p.Format)
            .Include(p => p.InputDescriptors).ThenInclude(p => p.Constraints)
            .SingleOrDefaultAsync(p => p.PublicId == id && p.RealmName == realm, cancellationToken);
        return result;
    }
}

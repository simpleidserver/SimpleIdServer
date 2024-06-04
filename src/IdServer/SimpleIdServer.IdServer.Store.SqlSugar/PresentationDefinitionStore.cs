// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class PresentationDefinitionStore : IPresentationDefinitionStore
{
    private readonly DbContext _dbContext;

    public PresentationDefinitionStore(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(PresentationDefinition presentationDefinition)
    {
        _dbContext.Client.InsertNav(SugarPresentationDefinition.Transform(presentationDefinition))
            .Include(p => p.InputDescriptors).ThenInclude(p => p.Format)
            .Include(p => p.InputDescriptors).ThenInclude(p => p.Constraints)
            .ExecuteCommand();
    }

    public async Task<List<PresentationDefinition>> GetAll(string realm, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarPresentationDefinition>()
            .Includes(p => p.InputDescriptors)
            .Where(p => p.RealmName == realm).ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }

    public async Task<PresentationDefinition> GetByPublicId(string id, string realm, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarPresentationDefinition>()
            .Includes(p => p.InputDescriptors, p => p.Format)
            .Includes(p => p.InputDescriptors, p => p.Constraints)
            .FirstAsync(p => p.PublicId == id && p.RealmName == realm, cancellationToken);
        return result?.ToDomain();
    }
}

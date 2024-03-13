// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store;

public interface IPresentationDefinitionStore
{
    IQueryable<PresentationDefinition> Query();
}

public class PresentationDefinitionStore : IPresentationDefinitionStore
{
    private readonly StoreDbContext _dbContext;

    public PresentationDefinitionStore(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IQueryable<PresentationDefinition> Query()
        => _dbContext.PresentationDefinitions;
}

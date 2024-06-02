// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit.Initializers;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class TranslationRepository : ITranslationRepository
{
    private readonly DbContext _dbContext;

    public TranslationRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Translation>> GetAllByKey(string key, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarTranslation>()
            .Where(t => t.Key == key)
            .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }
}

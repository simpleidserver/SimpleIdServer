// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class LanguageRepository : ILanguageRepository
{
    private readonly DbContext _dbContext;

    public LanguageRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Language>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarLanguage>().ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }
}

// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF;

public class LanguageRepository : ILanguageRepository
{
    private readonly StoreDbContext _dbContext;

    public LanguageRepository(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Language language)
    {
        _dbContext.Languages.Add(language);
    }

    public async Task<List<Language>> GetAll(CancellationToken cancellationToken)
    {
        var languages = await _dbContext.Languages.ToListAsync(cancellationToken);
        return languages;
    }
}

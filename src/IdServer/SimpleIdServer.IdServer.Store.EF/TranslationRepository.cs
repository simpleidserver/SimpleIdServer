// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Helpers.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF;

public class TranslationRepository : ITranslationRepository
{
    private readonly StoreDbContext _dbContext;

    public TranslationRepository(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Translation>> GetAllByKey(string key, CancellationToken cancellationToken)
        => _dbContext.Translations.Where(t => t.Key == key).ToListAsync(cancellationToken);
}
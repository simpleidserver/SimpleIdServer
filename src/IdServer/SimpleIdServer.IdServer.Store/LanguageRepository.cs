// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store;

public interface ILanguageRepository
{
    Task<List<Language>> GetAll(CancellationToken cancellationToken);
}

public class LanguageRepository : ILanguageRepository
{
    private readonly StoreDbContext _dbContext;

    public LanguageRepository(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Language>> GetAll(CancellationToken cancellationToken)
    {
        var languages = await _dbContext.Languages.ToListAsync(cancellationToken);
        return languages;
    }
}

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
        _dbContext.Translations.AddRange(language.Descriptions);
    }

    public async Task<List<Language>> GetAll(CancellationToken cancellationToken)
    {
        var languages = await _dbContext.Languages.ToListAsync(cancellationToken);
        foreach (var language in languages)
        {
            var descriptions = await _dbContext.Translations.Where(t => t.Key == language.TranslationKey).ToListAsync();
            language.Descriptions = descriptions;
            language.Description = GetDescription(language);
        }

        return languages;
    }

    private string GetDescription(Language language)
    {
        var description = language.Descriptions.SingleOrDefault(d => d.Key == language.TranslationKey && d.Language == Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName);
        return description?.Value;
    }
}

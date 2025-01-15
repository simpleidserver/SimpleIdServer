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

    public void Add(Language language)
    {
        _dbContext.Client.Insertable(SugarLanguage.Transform(language)).ExecuteCommand();
    }

    public async Task<List<Language>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarLanguage>().ToListAsync(cancellationToken);
        var res = result.Select(r => r.ToDomain()).ToList();
        foreach (var language in res)
        {
            var descriptions = await _dbContext.Client.Queryable<SugarTranslation>().Where(t => t.Key == language.TranslationKey).ToListAsync(cancellationToken);
            language.Descriptions = descriptions.Select(d => d.ToDomain()).ToList();
            language.Description = GetDescription(language);
        }

        return res;
    }

    private string GetDescription(Language language)
    {
        var description = language.Descriptions.SingleOrDefault(d => d.Key == language.TranslationKey && d.Language == Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName);
        return description?.Value;
    }
}

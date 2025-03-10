// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultLanguageRepository : ILanguageRepository
{
    private readonly List<Language> _languages;

    public DefaultLanguageRepository(List<Language> languages)
    {
        _languages = languages;
    }

    public void Add(Language language)
    {
        _languages.Add(language);
    }

    public Task<List<Language>> GetAll(CancellationToken cancellationToken)
    {
        foreach (var language in _languages)
        {
            language.Description = GetDescription(language);
        }

        return Task.FromResult(_languages.ToList());
    }

    private string GetDescription(Language language)
    {
        var currentLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        var description = language.Descriptions.SingleOrDefault(d => d.Language == currentLanguage);
        return description?.Value;
    }
}

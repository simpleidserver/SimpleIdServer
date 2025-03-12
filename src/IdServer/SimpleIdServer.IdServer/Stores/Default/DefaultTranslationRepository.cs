// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Helpers.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultTranslationRepository : ITranslationRepository
{
    private readonly List<Translation> _translations;

    public DefaultTranslationRepository(List<Translation> translations)
    {
        _translations = translations;
    }

    public Task<List<Translation>> GetAllByKey(string key, CancellationToken cancellationToken)
    {
        var result = _translations.Where(t => t.Key == key).ToList();
        return Task.FromResult(result);
    }
}

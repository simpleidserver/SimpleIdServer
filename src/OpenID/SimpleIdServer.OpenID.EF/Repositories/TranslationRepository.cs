// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.EF.Repositories
{
    public class TranslationRepository : ITranslationRepository
    {
        private readonly OpenIdDBContext _dbContext;

        public TranslationRepository(OpenIdDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<OAuthTranslation>> GetTranslations(IEnumerable<string> translationCodes, string language, CancellationToken cancellationToken)
        {
            IEnumerable<OAuthTranslation> result = await _dbContext.OAuthTranslation.Where(t => translationCodes.Contains(t.Key) && t.Language == language).ToListAsync(cancellationToken);
            return result;
        }

        public async Task<IEnumerable<OAuthTranslation>> GetTranslations(IEnumerable<string> translationCodes, CancellationToken cancellationToken)
        {
            IEnumerable<OAuthTranslation> result = await _dbContext.OAuthTranslation.Where(t => translationCodes.Contains(t.Key)).ToListAsync(cancellationToken);
            return result;
        }
    }
}

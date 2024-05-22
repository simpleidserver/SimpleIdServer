// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar
{
    public class TranslationRepository : ITranslationRepository
    {
        public Task<List<Translation>> GetAllByKey(string key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Translation> Query()
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

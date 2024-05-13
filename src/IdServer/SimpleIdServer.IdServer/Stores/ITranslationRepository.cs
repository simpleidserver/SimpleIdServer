// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores
{
    public interface ITranslationRepository
    {
        Task<List<Translation>> GetAllByKey(string key, CancellationToken cancellationToken);
        IQueryable<Translation> Query();
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}

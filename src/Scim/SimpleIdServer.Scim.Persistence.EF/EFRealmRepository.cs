// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Scim.Domains;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.EF
{
    public class EFRealmRepository : IRealmRepository
    {
        private readonly SCIMDbContext _dbContext;

        public EFRealmRepository(SCIMDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Realm> Get(string name, CancellationToken cancellationToken)
            => _dbContext.Realms.SingleOrDefaultAsync(r => r.Name == name, cancellationToken);
    }
}

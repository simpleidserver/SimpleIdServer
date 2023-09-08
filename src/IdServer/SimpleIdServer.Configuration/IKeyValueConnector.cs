// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Store;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Configuration
{
    public interface IKeyValueConnector
    {
        Task<Dictionary<string, string>> GetAll(CancellationToken cancellationToken);
    }

    public class EFKeyValueConnector : IKeyValueConnector
    {
        private readonly StoreDbContext _dbContext;

        public EFKeyValueConnector(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Dictionary<string, string>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _dbContext.ConfigurationKeyPairValueRecords.ToListAsync(cancellationToken);
            return result.ToDictionary(r => r.Name, r => r.Value);
        }
    }
}

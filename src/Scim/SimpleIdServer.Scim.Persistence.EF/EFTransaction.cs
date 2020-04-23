// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.EF
{
    public class EFTransaction : ITransaction
    {
        private readonly SCIMDbContext _dbContext;
        private readonly IDbContextTransaction _dbContextTransaction;

        public EFTransaction(SCIMDbContext scimDbContext, IDbContextTransaction dbContextTransaction)
        {
            _dbContext = scimDbContext;
            _dbContextTransaction = dbContextTransaction;
        }

        public async Task Commit(CancellationToken token)
        {
            await _dbContext.SaveChangesAsync(token);
            _dbContextTransaction.Commit();
        }

        public void Dispose()
        {
            _dbContextTransaction.Dispose();
        }
    }
}

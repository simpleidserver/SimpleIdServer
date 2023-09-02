// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using SimpleIdServer.Scim.Domains;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.EF
{
    public class EFTransaction : ITransaction
    {
        private readonly DbContext _dbContext;
        private readonly IDbContextTransaction _dbContextTransaction;

        public EFTransaction(DbContext scimDbContext, IDbContextTransaction dbContextTransaction)
        {
            _dbContext = scimDbContext;
            _dbContextTransaction = dbContextTransaction;
        }

        public async Task Commit(CancellationToken token)
        {
            var entries = _dbContext.ChangeTracker.Entries();
            var attrsToRemove = entries
                .Where(e => e.Entity.GetType() == typeof(SCIMRepresentationAttribute))
                .ToList();
            await _dbContext.SaveChangesAsync(token);
            await _dbContextTransaction.CommitAsync(token);
        }

        public void Dispose()
        {
            _dbContextTransaction.Dispose();
        }

        public ValueTask DisposeAsync() 
        {
            return _dbContextTransaction.DisposeAsync();
        }
    }
}

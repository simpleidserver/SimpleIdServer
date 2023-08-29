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
            ForceDelete();
            await _dbContext.SaveChangesAsync(token);
            await _dbContextTransaction.CommitAsync(token);
        }

        public void Dispose()
        {
            _dbContextTransaction.Dispose();
        }

        private void ForceDelete()
        {
            var entityEntryType = typeof(EntityEntry).GetProperty("InternalEntry", BindingFlags.Instance | BindingFlags.NonPublic);
            var entries = _dbContext.ChangeTracker.Entries();
#pragma warning disable EF1001
            var representations = entries
                .Where(e => e.State == EntityState.Modified && e.Entity.GetType() == typeof(SCIMRepresentation))
                .Select(e => entityEntryType.GetValue(e) as InternalEntityEntry)
                .ToList();
            var forceRemoveAttrs = entries.Where(e => e.State == EntityState.Modified && e.Entity.GetType() == typeof(SCIMRepresentationAttribute) && ((SCIMRepresentationAttribute)e.Entity).RepresentationId == null).ToList();
            foreach (var forceRemoveAttr in forceRemoveAttrs)
                forceRemoveAttr.State = EntityState.Deleted;

            var attrsToRemove = entries
                .Where(e => e.State == EntityState.Deleted && e.Entity.GetType() == typeof(SCIMRepresentationAttribute))
                .Select(e => entityEntryType.GetValue(e) as InternalEntityEntry)
                .ToList();
            var stateManagerProperty = typeof(ChangeTracker).GetProperty("StateManager", BindingFlags.NonPublic | BindingFlags.Instance);
            var stateManager = stateManagerProperty.GetValue(_dbContext.ChangeTracker) as IStateManager;
            var removeCollectionMethod = typeof(InternalEntityEntry).GetMethod("RemoveFromCollection", BindingFlags.Public | BindingFlags.Instance);
            foreach (var entry in representations)
            {
                var navigation = entry.EntityType.GetNavigations().Single(n => n.Name == "FlatAttributes");
                foreach (var attrToRemove in attrsToRemove) removeCollectionMethod.Invoke(entry, new object[] { navigation, attrToRemove });
            }
#pragma warning restore EF1001
        }

        public ValueTask DisposeAsync() 
        {
            return _dbContextTransaction.DisposeAsync();
        }
    }
}

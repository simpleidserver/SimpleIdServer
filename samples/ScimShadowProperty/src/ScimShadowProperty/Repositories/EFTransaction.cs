// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Persistence;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ScimShadowProperty.Repositories
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

        public DbContext DbContext => _dbContext;
        public IDbContextTransaction DbContextTransaction => _dbContextTransaction;

        public ICollection<string> BulkInsertedIds { get; } = new List<string>();

        public async virtual Task Commit(CancellationToken token)
        {
            ForceDelete();
            var entries = DbContext.ChangeTracker.Entries().Where(e => (e.State == EntityState.Added && (e.Entity.GetType() == typeof(SCIMRepresentation) || e.Entity.GetType() == typeof(SCIMRepresentationAttribute))) ||
                (e.Entity.GetType() == typeof(SCIMRepresentationAttribute) && BulkInsertedIds.Contains(((SCIMRepresentationAttribute)e.Entity).Id)));
            foreach (var entry in entries)
                entry.Property("Tenant").CurrentValue = "tenant";

            await _dbContext.SaveChangesAsync(token);
            await _dbContextTransaction.CommitAsync(token);
        }

        public void Dispose()
        {
            _dbContextTransaction.Dispose();
        }

        protected void ForceDelete()
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
                var representationId = (entry.Entity as SCIMRepresentation).Id;
                var navigation = entry.EntityType.GetNavigations().Single(n => n.Name == "FlatAttributes");
                foreach (var attrToRemove in attrsToRemove) removeCollectionMethod.Invoke(entry, new object[] { navigation, attrToRemove });
            }
#pragma warning restore EF1001
        }
    }
}

// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IAuditEventRepository
    {
        IQueryable<AuditEvent> Query();
        void Add(AuditEvent auditEvt);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class AuditEventRepository : IAuditEventRepository
    {
        private readonly StoreDbContext _dbContext;

        public AuditEventRepository(StoreDbContext dbContext) 
        { 
            _dbContext = dbContext;
        }

        public void Add(AuditEvent auditEvt) => _dbContext.AuditEvents.Add(auditEvt);

        public IQueryable<AuditEvent> Query() => _dbContext.AuditEvents;

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}

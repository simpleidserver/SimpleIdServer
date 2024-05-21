// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Api.Auditing;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Events;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Stores;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Store.EF;

public class AuditEventRepository : IAuditEventRepository
{
    private readonly StoreDbContext _dbContext;

    public AuditEventRepository(StoreDbContext dbContext) 
    { 
        _dbContext = dbContext;
    }

    public async Task<SearchResult<AuditEvent>> Search(string realm, SearchAuditingRequest request, CancellationToken cancellationToken)
    {
        var query = _dbContext.AuditEvents.AsNoTracking().Where(r => r.Realm == realm);
        if (request.DisplayOnlyErrors)
            query = query.Where(r => r.IsError);

        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);

        var nb = query.Count();
        var result = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync(cancellationToken);
        return new SearchResult<AuditEvent>
        {
            Content = result,
            Count = nb,
        };
    }

    public Task<int> NbInvalidAuthentications(string realm, DateTime startDateTime, CancellationToken cancellationToken)
    {
        return _dbContext.AuditEvents.AsNoTracking().CountAsync(e => e.CreateDateTime >= startDateTime && e.EventName == nameof(UserLoginFailureEvent) && e.Realm == realm);
    }

    public Task<int> NbValidAuthentications(string realm, DateTime startDateTime, CancellationToken cancellationToken)
    {
        return _dbContext.AuditEvents.AsNoTracking().CountAsync(e => e.CreateDateTime >= startDateTime && e.EventName == nameof(UserLoginSuccessEvent) && e.Realm == realm);
    }

    public void Add(AuditEvent auditEvt)
    {
        _dbContext.AuditEvents.Add(auditEvt);
    }
}

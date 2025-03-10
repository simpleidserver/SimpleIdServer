// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Api.Auditing;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultAuditEventRepository : IAuditEventRepository
{
    private readonly List<AuditEvent> _auditEvents;

    public DefaultAuditEventRepository(List<AuditEvent> auditEvents)
    {
        _auditEvents = auditEvents;
    }

    public async Task<SearchResult<AuditEvent>> Search(string realm, SearchAuditingRequest request, CancellationToken cancellationToken)
    {
        var query = _auditEvents.AsQueryable().Where(r => r.Realm == realm);

        if (request.DisplayOnlyErrors)
            query = query.Where(r => r.IsError);

        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);

        var nb = query.Count();
        var result = query.Skip(request.Skip.Value).Take(request.Take.Value).ToList();
        return await Task.FromResult(new SearchResult<AuditEvent>
        {
            Content = result,
            Count = nb
        });
    }

    public Task<int> NbInvalidAuthentications(string realm, DateTime startDateTime, CancellationToken cancellationToken)
    {
        var count = _auditEvents.AsQueryable().Count(e =>
            e.CreateDateTime >= startDateTime &&
            e.EventName == nameof(UserLoginFailureEvent) &&
            e.Realm == realm);
        return Task.FromResult(count);
    }

    public Task<int> NbValidAuthentications(string realm, DateTime startDateTime, CancellationToken cancellationToken)
    {
        var count = _auditEvents.AsQueryable().Count(e =>
            e.CreateDateTime >= startDateTime &&
            e.EventName == nameof(UserLoginSuccessEvent) &&
            e.Realm == realm);
        return Task.FromResult(count);
    }

    public void Add(AuditEvent auditEvt)
    {
        _auditEvents.Add(auditEvt);
    }
}

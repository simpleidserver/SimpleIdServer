// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Api.Auditing;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

internal class AuditEventRepository : IAuditEventRepository
{
    private readonly DbContext _dbContext;

    public AuditEventRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(AuditEvent auditEvt)
    {
        _dbContext.Client.Insertable(new SugarAuditEvent
        {
            UserName = auditEvt.UserName,
            RedirectUrl = auditEvt.RedirectUrl,
            AuthMethod = auditEvt.AuthMethod,
            ClientId = auditEvt.ClientId,
            CreateDateTime = auditEvt.CreateDateTime,
            Description = auditEvt.Description,
            ErrorMessage = auditEvt.ErrorMessage,
            EventName = auditEvt.EventName,
            Id = auditEvt.Id,
            IsError = auditEvt.IsError,
            Realm = auditEvt.Realm,
            RequestJSON = auditEvt.RequestJSON,
            Claims = auditEvt.Claims == null ? string.Empty : string.Join(",", auditEvt.Claims),
            Scopes = auditEvt.Scopes == null ? string.Empty : string.Join(",", auditEvt.Scopes)
        }).ExecuteCommand();
    }

    public async Task<int> NbInvalidAuthentications(string realm, DateTime startDateTime, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarAuditEvent>()
            .CountAsync(e => e.CreateDateTime >= startDateTime && e.EventName == nameof(UserLoginFailureEvent) && e.Realm == realm, cancellationToken);
        return result;
    }

    public async Task<int> NbValidAuthentications(string realm, DateTime startDateTime, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarAuditEvent>()
            .CountAsync(e => e.CreateDateTime >= startDateTime && e.EventName == nameof(UserLoginFailureEvent) && e.Realm == realm, cancellationToken);
        return result;
    }

    public  async Task<SearchResult<AuditEvent>> Search(string realm, SearchAuditingRequest request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Client.Queryable<SugarAuditEvent>()
            .Where(r => r.Realm == realm);
        if (request.DisplayOnlyErrors)
            query = query.Where(r => r.IsError);
        query = query.OrderByDescending(a => a.CreateDateTime);
        /*
        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);
        */

        var nb = query.Count();
        var result = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync(cancellationToken);
        return new SearchResult<AuditEvent>
        {
            Content = result.Select(r => r.ToDomain()).ToList(),
            Count = nb,
        };
    }
}

// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MassTransit.Initializers;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class UmaPendingRequestRepository : IUmaPendingRequestRepository
{
    private readonly DbContext _dbContext;

    public UmaPendingRequestRepository(DbContext dbContext)
    {
        _dbContext  = dbContext;
    }

    public void Add(UMAPendingRequest request)
    {
        _dbContext.Client.Insertable(Transform(request)).ExecuteCommand();
    }

    public void Update(UMAPendingRequest request)
    {
        _dbContext.Client.Updateable(Transform(request)).ExecuteCommand();
    }

    public async Task<List<UMAPendingRequest>> GetByPermissionTicketId(string permissionTicketId, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarUMAPendingRequest>()
            .Where(r => r.TicketId == permissionTicketId)
            .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }

    public async Task<List<UMAPendingRequest>> GetByUsername(string realm, string userName, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarUMAPendingRequest>()
            .Includes(p => p.Resource, p => p.Translations)
            .Where(r => (r.Owner == userName || r.Requester == userName) && r.Realm == realm)
            .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }

    private static SugarUMAPendingRequest Transform(UMAPendingRequest request)
    {
        return new SugarUMAPendingRequest
        {
            CreateDateTime = request.CreateDateTime,
            TicketId = request.TicketId,
            Owner = request.Owner,
            Realm = request.Realm,
            Requester = request.Requester,
            Scopes = request.Scopes == null ? string.Empty : string.Join(",", request.Scopes),
            Status = request.Status
        };
    }
}

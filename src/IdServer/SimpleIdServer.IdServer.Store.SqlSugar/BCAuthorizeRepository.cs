// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit.Initializers;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class BCAuthorizeRepository : IBCAuthorizeRepository
{
    private readonly DbContext _dbContext;

    public BCAuthorizeRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(BCAuthorize bcAuthorize)
    {
        _dbContext.Client.InsertNav(Transform(bcAuthorize))
            .Include(b => b.Histories)
            .ExecuteCommand();
    }

    public void Update(BCAuthorize bcAuthorize)
    {
        _dbContext.Client.UpdateNav(Transform(bcAuthorize))
            .Include(b => b.Histories)
            .ExecuteCommand();
    }

    public async Task<List<BCAuthorize>> GetAllConfirmed(List<string> notificationModes, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarBCAuthorize>()
            .Includes(a => a.Histories)
            .Where(a => a.LastStatus == Domains.BCAuthorizeStatus.Confirmed && notificationModes.Contains(a.NotificationMode) && DateTime.UtcNow < a.ExpirationDateTime)
            .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }

    public async Task<BCAuthorize> GetById(string id, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarBCAuthorize>()
            .Includes(a => a.Histories)
            .FirstAsync(b => b.Id == id, cancellationToken);
        return result?.ToDomain();
    }

    private SugarBCAuthorize Transform(BCAuthorize bcAuthorize)
    {
        return new SugarBCAuthorize
        {
            ClientId = bcAuthorize.ClientId,
            ExpirationDateTime = bcAuthorize.ExpirationDateTime,
            Id = bcAuthorize.Id,
            Interval = bcAuthorize.Interval,
            LastStatus = bcAuthorize.LastStatus,
            NextFetchTime = bcAuthorize.NextFetchTime,
            NotificationEdp = bcAuthorize.NotificationEdp,
            NotificationMode = bcAuthorize.NotificationMode,
            NotificationToken = bcAuthorize.NotificationToken,
            Realm = bcAuthorize.Realm,
            RejectionSentDateTime = bcAuthorize.RejectionSentDateTime,
            Scopes = bcAuthorize.Scopes == null ? string.Empty : string.Join(",", bcAuthorize.Scopes),
            SerializedAuthorizationDetails = bcAuthorize.SerializedAuthorizationDetails,
            UpdateDateTime = bcAuthorize.UpdateDateTime,
            UserId = bcAuthorize.UserId,
            Histories = bcAuthorize.Histories.Select(h => new SugarBCAuthorizeHistory
            {
                Message = h.Message,
                EndDateTime = h.EndDateTime,
                StartDateTime = h.StartDateTime,
                Status = h.Status
            }).ToList()
        };
    }
}

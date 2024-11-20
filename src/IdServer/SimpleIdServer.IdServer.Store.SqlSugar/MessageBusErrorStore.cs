// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class MessageBusErrorStore : IMessageBusErrorStore
{
    private readonly DbContext _dbContext;

    public MessageBusErrorStore(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(MessageBusErrorMessage message)
    {
        _dbContext.Client.Insertable(SugarMessageBusErrorMessage.Transform(message))
            .ExecuteCommand();
    }

    public void Delete(MessageBusErrorMessage message)
    {
        _dbContext.Client.Deleteable(SugarMessageBusErrorMessage.Transform(message))
            .ExecuteCommand();
    }

    public async Task<MessageBusErrorMessage> Get(string id, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarMessageBusErrorMessage>()
            .SingleAsync(r => r.Id == id);
        return result?.ToDomain();
    }

    public async Task<List<MessageBusErrorMessage>> GetAllByExternalId(List<string> externalIds, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarMessageBusErrorMessage>()
            .Where(r => externalIds.Contains(r.ExternalId))
            .ToListAsync();
        return result.Select(r => r.ToDomain()).ToList();
    }
}

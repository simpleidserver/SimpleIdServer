// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF
{
    public class MessageBusErrorStore : IMessageBusErrorStore
    {
        private readonly StoreDbContext _dbContext;

        public MessageBusErrorStore(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(MessageBusErrorMessage message)
        {
            _dbContext.MessageBusErrorMessages.Add(message);
        }

        public void Delete(MessageBusErrorMessage message)
        {
            _dbContext.MessageBusErrorMessages.Remove(message);
        }

        public Task<MessageBusErrorMessage> Get(string id, CancellationToken cancellationToken)
            => _dbContext.MessageBusErrorMessages.SingleOrDefaultAsync(m => m.Id == id, cancellationToken);

        public Task<List<MessageBusErrorMessage>> GetAllByExternalId(List<string> externalIds, CancellationToken cancellationToken) =>
            _dbContext.MessageBusErrorMessages.Where(m => externalIds.Contains(m.ExternalId)).ToListAsync(cancellationToken);
    }
}

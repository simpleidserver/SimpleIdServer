// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IPollingDeviceMessageRepository
    {
        IQueryable<PollingDeviceMessage> Query();
        void Add(PollingDeviceMessage message);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class PollingDeviceMessageRepository : IPollingDeviceMessageRepository
    {
        private readonly StoreDbContext _dbContext;

        public PollingDeviceMessageRepository(StoreDbContext dbContext)
        {
            _dbContext= dbContext;
        }

        public IQueryable<PollingDeviceMessage> Query() => _dbContext.PollingDeviceMessages;

        public void Add(PollingDeviceMessage message) => _dbContext.PollingDeviceMessages.Add(message);

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}

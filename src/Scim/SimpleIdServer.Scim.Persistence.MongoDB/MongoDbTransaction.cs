// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.MongoDB
{
    public class MongoDbTransaction : ITransaction
    {
        private readonly IClientSessionHandle _clientSessionHandle;

        public MongoDbTransaction()
        {

        }

        public MongoDbTransaction(IClientSessionHandle clientSessionHandle)
        {
            _clientSessionHandle = clientSessionHandle;
        }

        public Task Commit(CancellationToken token)
        {
            if (_clientSessionHandle != null)
            {
                return _clientSessionHandle.CommitTransactionAsync(token);
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_clientSessionHandle != null)
            {
                _clientSessionHandle.Dispose();
            }
        }
    }
}

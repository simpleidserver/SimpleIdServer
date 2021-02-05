// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Extensions;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Persistence.InMemory
{
    public class DefaultTokenCommandRepository : ITokenCommandRepository
    {
        private readonly ConcurrentBag<Token> _tokens;

        public DefaultTokenCommandRepository(ConcurrentBag<Token> tokens)
        {
            _tokens = tokens;
        }

        public Task<bool> Add(Token token, CancellationToken cancellationToken)
        {
            _tokens.Add(token);
            return Task.FromResult(true);
        }

        public Task<bool> Delete(Token token, CancellationToken cancellationToken)
        {
            var record = _tokens.FirstOrDefault(_ => _.Data == token.Data);
            if (record == null)
            {
                return Task.FromResult(false);
            }

            _tokens.Remove(record);
            return Task.FromResult(true);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}

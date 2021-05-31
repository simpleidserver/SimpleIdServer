// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Persistence.Parameters;
using SimpleIdServer.OAuth.Persistence.Results;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Persistence.InMemory
{
    public class DefaultTokenRepository : ITokenRepository
    {
        private readonly ConcurrentBag<Token> _tokens;

        public DefaultTokenRepository(ConcurrentBag<Token> tokens)
        {
            _tokens = tokens;
        }

        public Task<SearchResult<Token>> Find(SearchTokenParameter parameter, CancellationToken token)
        {
            var filtered = _tokens.AsQueryable();
            if (!string.IsNullOrWhiteSpace(parameter.TokenType))
            {
                filtered = filtered.Where(_ => _.TokenType == parameter.TokenType);
            }

            if (!string.IsNullOrWhiteSpace(parameter.AuthorizationCode))
            {
                filtered = filtered.Where(_ => _.AuthorizationCode == parameter.AuthorizationCode);
            }

            if (!string.IsNullOrWhiteSpace(parameter.ClientId))
            {
                filtered = filtered.Where(_ => _.ClientId == parameter.ClientId);
            }

            filtered = filtered.OrderByDescending(_ => _.CreateDateTime);
            int totalResult = filtered.Count();
            var result = filtered.Skip(parameter.StartIndex).Take(parameter.Count);
            return Task.FromResult(new SearchResult<Token>
            {
                Content = result.ToList(),
                TotalLength = totalResult,
                StartIndex = parameter.StartIndex,
                Count = result.Count()
            });
        }

        public Task<Token> Get(string id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_tokens.FirstOrDefault(_ => _.Id == id));
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

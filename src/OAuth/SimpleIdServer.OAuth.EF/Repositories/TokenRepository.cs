// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OAuth.Persistence.Parameters;
using SimpleIdServer.OAuth.Persistence.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.EF.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly OAuthDBContext _dbContext;

        public TokenRepository(OAuthDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SearchResult<Token>> Find(SearchTokenParameter parameter, CancellationToken token)
        {
            IQueryable<Token> filtered = _dbContext.Tokens;
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
            int totalResult = await filtered.CountAsync(token);
            var result = await filtered.Skip(parameter.StartIndex).Take(parameter.Count).ToListAsync(token);
            return new SearchResult<Token>
            {
                Content = result,
                TotalLength = totalResult,
                StartIndex = parameter.StartIndex,
                Count = result.Count()
            };
        }

        public Task<Token> Get(string id, CancellationToken cancellationToken)
        {
            return _dbContext.Tokens.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public Task<bool> Add(Token token, CancellationToken cancellationToken)
        {
            _dbContext.Tokens.Add(token);
            return Task.FromResult(true);
        }

        public Task<bool> Delete(Token token, CancellationToken cancellationToken)
        {
            _dbContext.Tokens.Remove(token);
            return Task.FromResult(true);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

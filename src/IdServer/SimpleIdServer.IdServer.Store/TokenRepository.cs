// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface ITokenRepository
    {
        IQueryable<Token> Query();
        void Add(Token token);
        void Remove(Token token);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class TokenRepository : ITokenRepository
    {
        private readonly StoreDbContext _dbContext;

        public TokenRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Token> Query() => _dbContext.Tokens;

        public void Add(Token token) => _dbContext.Tokens.Add(token);

        public void Remove(Token token) => _dbContext.Tokens.Remove(token);

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}

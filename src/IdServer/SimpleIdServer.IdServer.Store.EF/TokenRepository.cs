// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF
{
    public class TokenRepository : ITokenRepository
    {
        private readonly StoreDbContext _dbContext;

        public TokenRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Token> Get(string id, CancellationToken cancellationToken)
            => _dbContext.Tokens.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        public Task<List<Token>> GetByGrantId(string grantId, CancellationToken cancellationToken)
            => _dbContext.Tokens.Where(t => t.GrantId == grantId).ToListAsync(cancellationToken);

        public IQueryable<Token> Query() => _dbContext.Tokens;

        public void Add(Token token) => _dbContext.Tokens.Add(token);

        public void Remove(Token token) => _dbContext.Tokens.Remove(token);

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}

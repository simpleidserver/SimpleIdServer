// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class TokenRepository : ITokenRepository
{
    public void Add(Token token)
    {
        throw new NotImplementedException();
    }

    public Task<Token> Get(string id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<Token>> GetAllByAuthorizationCode(string authorizationCode, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<Token>> GetByGrantId(string grantId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IQueryable<Token> Query()
    {
        throw new NotImplementedException();
    }

    public void Remove(Token token)
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

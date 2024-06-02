// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class TokenRepository : ITokenRepository
{
    private readonly DbContext _dbContext;

    public TokenRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Token token)
    {
        _dbContext.Client.Insertable(Transform(token)).ExecuteCommand();
    }

    public async Task<Token> Get(string id, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarToken>()
            .FirstAsync(t => t.Id == id, cancellationToken);
        return result?.ToDomain();
    }

    public async Task<List<Token>> GetAllByAuthorizationCode(string authorizationCode, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarToken>()
            .Where(t => t.AuthorizationCode == authorizationCode)
            .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }

    public async Task<List<Token>> GetByGrantId(string grantId, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarToken>()
            .Where(t => t.GrantId == grantId)
            .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }

    public void Remove(Token token)
    {
        _dbContext.Client.Deleteable(Transform(token)).ExecuteCommand();
    }

    private SugarToken Transform(Token token)
    {
        return new SugarToken
        {
            AccessTokenType = token.AccessTokenType,
            AuthorizationCode = token.AuthorizationCode,
            GrantId = token.GrantId,
            ClientId = token.ClientId,
            CreateDateTime = token.CreateDateTime,
            Data = token.Data,
            ExpirationTime = token.ExpirationTime,
            Id = token.Id,
            Jkt = token.Jkt,
            OriginalData = token.OriginalData,
            SessionId = token.SessionId,
            TokenType = token.TokenType
        };
    }
}

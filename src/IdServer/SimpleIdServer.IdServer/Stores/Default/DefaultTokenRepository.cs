// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultTokenRepository : ITokenRepository
{
    private readonly List<Token> _tokens;

    public DefaultTokenRepository(List<Token> tokens)
    {
        _tokens = tokens;
    }

    public Task<Token> Get(string id, CancellationToken cancellationToken)
        => Task.FromResult(_tokens.FirstOrDefault(t => t.Id == id));

    public Task<List<Token>> GetAllByAuthorizationCode(string authorizationCode, CancellationToken cancellationToken)
        => Task.FromResult(_tokens.Where(t => t.AuthorizationCode == authorizationCode).ToList());

    public Task<List<Token>> GetByGrantId(string grantId, CancellationToken cancellationToken)
        => Task.FromResult(_tokens.Where(t => t.GrantId == grantId).ToList());

    public IQueryable<Token> Query() => _tokens.AsQueryable();

    public void Add(Token token) => _tokens.Add(token);

    public void Remove(Token token) => _tokens.Remove(token);
}

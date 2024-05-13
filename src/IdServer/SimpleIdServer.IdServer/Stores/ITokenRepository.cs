// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface ITokenRepository
{
    Task<Token> Get(string id, CancellationToken cancellationToken);
    Task<List<Token>> GetByGrantId(string grantId, CancellationToken cancellationToken);
    IQueryable<Token> Query();
    void Add(Token token);
    void Remove(Token token);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}

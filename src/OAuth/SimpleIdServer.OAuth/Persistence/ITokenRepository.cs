﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Persistence.Parameters;
using SimpleIdServer.OAuth.Persistence.Results;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Persistence
{
    public interface ITokenRepository
    {
        Task<bool> Add(Token token, CancellationToken cancellationToken);
        Task<bool> Delete(Token token, CancellationToken cancellationToken);
        Task<int> SaveChanges(CancellationToken cancellationToken);
        Task<Token> Get(string id, CancellationToken cancellationToken);
        Task<SearchResult<Token>> Find(SearchTokenParameter parameter, CancellationToken token);
    }
}

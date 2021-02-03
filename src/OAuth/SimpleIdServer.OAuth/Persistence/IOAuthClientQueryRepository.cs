// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Persistence.Parameters;
using SimpleIdServer.OAuth.Persistence.Results;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Persistence
{
    public interface IOAuthClientQueryRepository
    {
        Task<OAuthClient> FindOAuthClientById(string clientId);
        Task<IEnumerable<OAuthClient>> FindOAuthClientByIds(IEnumerable<string> clientIds, CancellationToken token);
        Task<SearchResult<OAuthClient>> Find(SearchClientParameter parameter, CancellationToken token);
    }
}
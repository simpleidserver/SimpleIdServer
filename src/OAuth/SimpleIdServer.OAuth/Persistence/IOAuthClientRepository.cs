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
    public interface IOAuthClientRepository : ICommandRepository<BaseClient>
    {
        Task<BaseClient> FindOAuthClientById(string clientId, CancellationToken cancellationToken);
        Task<IEnumerable<BaseClient>> FindOAuthClientByIds(IEnumerable<string> clientIds, CancellationToken token);
        Task<SearchResult<BaseClient>> Find(SearchClientParameter parameter, CancellationToken token);
        Task<List<string>> GetResources(IEnumerable<string> names, CancellationToken cancellationToken);
    }
}
// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.Uma.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Persistence
{
    public interface IUMAResourceRepository : ICommandRepository<UMAResource>
    {
        Task<IEnumerable<UMAResource>> GetAll(CancellationToken cancellationToken);
        Task<SearchResult<UMAResource>> Find(SearchUMAResourceParameter searchUMAResourceParameter, CancellationToken cancellationToken);
        Task<IEnumerable<UMAResource>> FindByIdentifiers(IEnumerable<string> ids, CancellationToken cancellationToken);
        Task<UMAResource> FindByIdentifier(string id, CancellationToken cancellationToken);
    }
}

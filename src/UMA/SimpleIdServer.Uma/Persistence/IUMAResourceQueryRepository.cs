// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Uma.Domains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Persistence
{
    public interface IUMAResourceQueryRepository
    {
        Task<IEnumerable<UMAResource>> GetAll();
        Task<SearchResult<UMAResource>> Find(SearchUMAResourceParameter searchUMAResourceParameter);
        Task<IEnumerable<UMAResource>> FindByIdentifiers(IEnumerable<string> ids);
        Task<UMAResource> FindByIdentifier(string id);
    }
}

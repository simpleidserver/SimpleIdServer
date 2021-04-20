// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OpenID.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Persistence
{
    public interface IBCAuthorizeRepository
    {
        Task<IEnumerable<BCAuthorize>> GetConfirmedAuthorizationRequest(CancellationToken cancellationToken);
        Task<BCAuthorize> Get(string id, CancellationToken cancellationToken);
        Task Add(BCAuthorize bcAuthorize, CancellationToken cancellationToken);
        Task Update(BCAuthorize bcAuhtorize, CancellationToken cancellationToken);
        Task Delete(BCAuthorize bcAuthorize, CancellationToken cancellationToken);
        Task SaveChanges(CancellationToken cancellationToken);
    }
}

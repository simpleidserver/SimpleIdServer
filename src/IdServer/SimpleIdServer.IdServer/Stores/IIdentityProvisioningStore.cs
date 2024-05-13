// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IIdentityProvisioningStore
{
    Task<SearchResult<IdentityProvisioning>> Search(string realm, SearchRequest request, CancellationToken cancellationToken);
    Task<IdentityProvisioning> Get(string realm, string id, CancellationToken cancellationToken);
    IQueryable<IdentityProvisioning> Query();
    void DeleteRange(IEnumerable<IdentityProvisioning> identityProvisioningLst);
    void Remove(IdentityProvisioning identityProvisioning);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}

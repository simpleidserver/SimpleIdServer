// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IIdentityProvisioningStore
{
    Task<SearchResult<IdentityProvisioning>> Search(string realm, SearchRequest request, CancellationToken cancellationToken);
    Task<IdentityProvisioning> Get(string realm, string id, CancellationToken cancellationToken);
    Task<IdentityProvisioning> GetByName(string realm, string name, CancellationToken cancellationToken);
    void DeleteRange(IEnumerable<IdentityProvisioning> identityProvisioningLst);
    void Add(IdentityProvisioning identityProvisioning);
    void Remove(IdentityProvisioning identityProvisioning);
    void Update(IdentityProvisioning identityProvisioning);
    Task<IdentityProvisioningDefinition> GetDefinitionByName(string name, CancellationToken cancellationToken);
    void Add(IdentityProvisioningDefinition identityProvisioning);
    void Update(IdentityProvisioningDefinition identityProvisioningDefinition);
}

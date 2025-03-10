// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultIdentityProvisioningStore : IIdentityProvisioningStore
{
    private readonly List<IdentityProvisioningDefinition> _definitions;

    public DefaultIdentityProvisioningStore(List<IdentityProvisioningDefinition> definitions)
    {
        _definitions = definitions;
    }

    public Task<SearchResult<IdentityProvisioning>> Search(string realm, SearchRequest request, CancellationToken cancellationToken)
    {
        var allProvisionings = _definitions.SelectMany(d => d.Instances)
            .Where(p => p.Realms.Any(r => r.Name == realm));
        if (!string.IsNullOrWhiteSpace(request.Filter))
            allProvisionings = allProvisionings.Where(p => !string.IsNullOrEmpty(p.Name) && p.Name.Contains(request.Filter, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(request.OrderBy) && request.OrderBy.Equals("name", StringComparison.OrdinalIgnoreCase))
            allProvisionings = allProvisionings.OrderBy(p => p.Name);
        var list = allProvisionings.ToList();
        var skip = request.Skip ?? 0;
        var take = request.Take ?? list.Count;
        var result = new SearchResult<IdentityProvisioning>
        {
            Count = list.Count,
            Content = list.Skip(skip).Take(take).ToList()
        };
        return Task.FromResult(result);
    }

    public Task<IdentityProvisioning> Get(string realm, string id, CancellationToken cancellationToken)
    {
        var provisioning = _definitions.SelectMany(d => d.Instances)
            .FirstOrDefault(p => p.Id == id && p.Realms.Any(r => r.Name == realm));
        return Task.FromResult(provisioning);
    }

    public void DeleteRange(IEnumerable<IdentityProvisioning> identityProvisioningLst)
    {
        foreach (var provisioning in identityProvisioningLst)
            Remove(provisioning);
    }

    public void Remove(IdentityProvisioning identityProvisioning)
    {
        foreach (var definition in _definitions)
            if (definition.Instances.Contains(identityProvisioning))
            {
                definition.Instances.Remove(identityProvisioning);
                break;
            }
    }

    public void Update(IdentityProvisioning identityProvisioning)
    {
    }

    public void Add(IdentityProvisioningDefinition identityProvisioningDefinition)
    {
        _definitions.Add(identityProvisioningDefinition);
    }

    public void Update(IdentityProvisioningDefinition identityProvisioningDefinition)
    {
    }
}

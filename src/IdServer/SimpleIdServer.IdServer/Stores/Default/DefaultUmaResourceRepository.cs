// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultUmaResourceRepository : IUmaResourceRepository
{
    private readonly List<UMAResource> _umaResources;

    public DefaultUmaResourceRepository(List<UMAResource> umaResources)
    {
        _umaResources = umaResources;
    }

    public Task<UMAResource> Get(string id, CancellationToken cancellationToken)
    {
        var resource = _umaResources.FirstOrDefault(r => r.Id == id);
        return Task.FromResult(resource);
    }

    public Task<List<UMAResource>> GetByIds(List<string> resourceIds, CancellationToken cancellationToken)
    {
        var resources = _umaResources.Where(r => resourceIds.Contains(r.Id)).ToList();
        return Task.FromResult(resources);
    }

    public Task<List<UMAResource>> GetAll(CancellationToken cancellationToken)
    {
        return Task.FromResult(_umaResources.ToList());
    }

    public void Add(UMAResource resource) => _umaResources.Add(resource);

    public void Delete(UMAResource resource) => _umaResources.Remove(resource);

    public void Update(UMAResource resource)
    {
        var index = _umaResources.FindIndex(r => r.Id == resource.Id);
        if (index != -1)
            _umaResources[index] = resource;
    }
}

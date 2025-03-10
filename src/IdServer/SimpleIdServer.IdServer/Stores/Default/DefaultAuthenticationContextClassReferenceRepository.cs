// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultAuthenticationContextClassReferenceRepository : IAuthenticationContextClassReferenceRepository
{
    private readonly List<AuthenticationContextClassReference> _acrs;

    public DefaultAuthenticationContextClassReferenceRepository(List<AuthenticationContextClassReference> acrs)
    {
        _acrs = acrs;
    }

    public Task<AuthenticationContextClassReference> Get(string realm, string id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_acrs
            .SingleOrDefault(a => a.Realms.Any(r => r.Name == realm) && a.Id == id));
    }

    public Task<AuthenticationContextClassReference> GetByAuthenticationWorkflow(string realm, string workflowId, CancellationToken cancellationToken)
    {
        return Task.FromResult(_acrs
            .SingleOrDefault(a => a.Realms.Any(r => r.Name == realm) && a.AuthenticationWorkflow == workflowId));
    }

    public Task<AuthenticationContextClassReference> GetByName(string realm, string name, CancellationToken cancellationToken)
    {
        return Task.FromResult(_acrs
            .SingleOrDefault(a => a.Realms.Any(r => r.Name == realm) && a.Name == name));
    }

    public Task<List<AuthenticationContextClassReference>> GetByNames(List<string> names, CancellationToken cancellationToken)
    {
        return Task.FromResult(_acrs
            .Where(a => names.Contains(a.Name))
            .ToList());
    }

    public Task<List<AuthenticationContextClassReference>> GetByNames(string realm, List<string> names, CancellationToken cancellationToken)
    {
        return Task.FromResult(_acrs
            .Where(a => names.Contains(a.Name) && a.Realms.Any(r => r.Name == realm))
            .ToList());
    }

    public Task<List<AuthenticationContextClassReference>> GetAll(CancellationToken cancellationToken)
    {
        return Task.FromResult(_acrs.ToList());
    }

    public Task<List<AuthenticationContextClassReference>> GetAll(string realm, CancellationToken cancellationToken)
    {
        return Task.FromResult(_acrs
            .Where(a => a.Realms.Any(r => r.Name == realm))
            .OrderBy(a => a.Name)
            .ToList());
    }

    public void Add(AuthenticationContextClassReference record) => _acrs.Add(record);

    public void Delete(AuthenticationContextClassReference record) => _acrs.Remove(record);

    public void Update(AuthenticationContextClassReference record)
    {
        var existingRecord = _acrs.SingleOrDefault(a => a.Id == record.Id);
        if (existingRecord != null)
        {
            _acrs.Remove(existingRecord);
            _acrs.Add(record);
        }
    }
}

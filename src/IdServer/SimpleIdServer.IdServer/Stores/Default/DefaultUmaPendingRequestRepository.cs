// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultUmaPendingRequestRepository : IUmaPendingRequestRepository
{
    private readonly List<UMAPendingRequest> _umaPendingRequests;

    public DefaultUmaPendingRequestRepository(List<UMAPendingRequest> umaPendingRequests)
    {
        _umaPendingRequests = umaPendingRequests;
    }

    public Task<List<UMAPendingRequest>> GetByPermissionTicketId(string permissionTicketId, CancellationToken cancellationToken)
    {
        var result = _umaPendingRequests.Where(r => r.TicketId == permissionTicketId).ToList();
        return Task.FromResult(result);
    }

    public Task<List<UMAPendingRequest>> GetByUsername(string realm, string userName, CancellationToken cancellationToken)
    {
        var result = _umaPendingRequests
            .Where(r => (r.Owner == userName || r.Requester == userName) && r.Realm == realm)
            .ToList();
        return Task.FromResult(result);
    }

    public void Add(UMAPendingRequest request) => _umaPendingRequests.Add(request);

    public void Update(UMAPendingRequest request)
    {
        var index = _umaPendingRequests.FindIndex(r => r.TicketId == request.TicketId);
        if (index >= 0)
        {
            _umaPendingRequests[index] = request;
        }
    }
}

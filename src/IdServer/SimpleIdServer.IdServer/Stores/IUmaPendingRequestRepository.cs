// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IUmaPendingRequestRepository
{
    Task<List<UMAPendingRequest>> GetByPermissionTicketId(string permissionTicketId, CancellationToken cancellationToken);
    Task<List<UMAPendingRequest>> GetByUsername(string realm, string userName, CancellationToken cancellationToken);
    void Add(UMAPendingRequest request);
    void Update(UMAPendingRequest request);
}

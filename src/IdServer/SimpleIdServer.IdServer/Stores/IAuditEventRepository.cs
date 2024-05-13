// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Api.Auditing;
using SimpleIdServer.IdServer.Domains;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IAuditEventRepository
{
    Task<SearchResult<AuditEvent>> Search(string realm, SearchAuditingRequest request, CancellationToken cancellationToken);
}
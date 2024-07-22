// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Api.Auditing;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IAuditEventRepository
{
    Task<SearchResult<AuditEvent>> Search(string realm, SearchAuditingRequest request, CancellationToken cancellationToken);
    Task<int> NbValidAuthentications(string realm, DateTime startDateTime, CancellationToken cancellationToken);
    Task<int> NbInvalidAuthentications(string realm, DateTime startDateTime, CancellationToken cancellationToken);
    void Add(AuditEvent auditEvt);
}
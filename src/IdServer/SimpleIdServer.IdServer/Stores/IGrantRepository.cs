// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IGrantRepository
{
    Task<Consent> Get(string id, CancellationToken cancellation);
    IQueryable<Consent> Query();
    void Remove(Consent consent);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}

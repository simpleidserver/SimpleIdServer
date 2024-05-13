// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IFileSerializedKeyStore
{
    IQueryable<SerializedFileKey> Query();
    Task<List<SerializedFileKey>> GetAll(string realm, CancellationToken cancellationToken);
    void Add(SerializedFileKey key);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}

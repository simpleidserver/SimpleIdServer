// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class SerializedFileKeyStore : IFileSerializedKeyStore
{
    public void Add(SerializedFileKey key)
    {
        throw new NotImplementedException();
    }

    public Task<List<SerializedFileKey>> GetAll(string realm, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<SerializedFileKey>> GetAllEnc(string realm, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<SerializedFileKey>> GetAllSig(string realm, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IQueryable<SerializedFileKey> Query()
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

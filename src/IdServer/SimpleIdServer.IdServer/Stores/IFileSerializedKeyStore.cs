// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IFileSerializedKeyStore
{
    Task<List<SerializedFileKey>> GetAll(string realm, CancellationToken cancellationToken);
    Task<List<SerializedFileKey>> GetAllSig(string realm, CancellationToken cancellationToken);
    Task<List<SerializedFileKey>> GetAllEnc(string realm, CancellationToken cancellationToken);
    void Add(SerializedFileKey key);
    void Update(SerializedFileKey key);
}

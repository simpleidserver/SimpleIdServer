// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.WalletClient.Stores;

public interface IVcStore
{
    Task<List<StoredVcRecord>> GetAll(CancellationToken cancellationToken);
}

public class VcStore : IVcStore
{
    private readonly List<StoredVcRecord> _storedVcRecords;
    
    public VcStore()
    {
        
    }

    public VcStore(List<StoredVcRecord> storedVcRecords)
    {
        _storedVcRecords = storedVcRecords;
    }

    public Task<List<StoredVcRecord>> GetAll(CancellationToken cancellationToken)
    {
        return Task.FromResult(_storedVcRecords);
    }
}

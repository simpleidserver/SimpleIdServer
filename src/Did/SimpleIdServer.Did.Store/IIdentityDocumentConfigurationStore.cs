// Copyright(c) SimpleIdServer.All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Did.Store
{
    public interface IIdentityDocumentConfigurationStore
    {
        Task<NetworkConfiguration> Get(string name, CancellationToken cancellationToken);
        void Add(NetworkConfiguration networkConfiguration);
        void Remove(NetworkConfiguration networkConfiguration);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}

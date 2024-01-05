// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Ethr.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Did.Ethr.Stores;

public interface INetworkConfigurationStore
{
    Task<NetworkConfiguration> Get(string name, CancellationToken cancellationToken);
}

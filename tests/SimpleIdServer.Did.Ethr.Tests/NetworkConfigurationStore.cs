// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Ethr.Models;
using SimpleIdServer.Did.Ethr.Stores;

namespace SimpleIdServer.Did.Ethr.Tests;

public class NetworkConfigurationStore : INetworkConfigurationStore
{
    public List<NetworkConfiguration> NetworkConfigurations = new List<NetworkConfiguration>();

    public Task<NetworkConfiguration> Get(string name, CancellationToken cancellationToken)
        => Task.FromResult(NetworkConfigurations.Single(c => c.Name == name));
}

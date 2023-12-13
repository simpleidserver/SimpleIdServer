// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;

namespace SimpleIdServer.Did.Ethr.Tests
{
    public class MockIdentityConfigurationStore : IIdentityDocumentConfigurationStore
    {
        private readonly ICollection<NetworkConfiguration> _configurations = SimpleIdServer.Did.Ethr.Constants.StandardNetworkConfigurations;

        public void Add(NetworkConfiguration networkConfiguration) => _configurations.Add(networkConfiguration);

        public IQueryable<NetworkConfiguration> Query() => _configurations.AsQueryable();

        public void Remove(NetworkConfiguration networkConfiguration) => _configurations.Remove(networkConfiguration);

        public Task<int> SaveChanges(CancellationToken cancellationToken) => Task.FromResult(1);
    }
}

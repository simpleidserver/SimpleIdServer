// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Models;
// using SimpleIdServer.Did.Store;

/*
namespace SimpleIdServer.Did.Ethr.Tests
{
    public class MockIdentityConfigurationStore : IIdentityDocumentConfigurationStore
    {
        private readonly ICollection<NetworkConfiguration> _configurations = SimpleIdServer.Did.Ethr.Constants.StandardNetworkConfigurations;

        public void Add(NetworkConfiguration networkConfiguration) => _configurations.Add(networkConfiguration);

        public Task<NetworkConfiguration> Get(string name, CancellationToken cancellationToken) => Task.FromResult(_configurations.SingleOrDefault(c => c.Name == name));

        public IQueryable<NetworkConfiguration> Query() => _configurations.AsQueryable();

        public void Remove(NetworkConfiguration networkConfiguration) => _configurations.Remove(networkConfiguration);

        public Task<int> SaveChanges(CancellationToken cancellationToken) => Task.FromResult(1);
    }
}
*/
// Copyright(c) SimpleIdServer.All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Ethr.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Did.Ethr
{
    public interface IIdentityDocumentConfigurationStore
    {
        IQueryable<NetworkConfiguration> Query();
        void Add(NetworkConfiguration networkConfiguration);
        void Remove(NetworkConfiguration networkConfiguration);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class IdentityDocumentConfigurationStore : IIdentityDocumentConfigurationStore
    {
        private readonly ICollection<NetworkConfiguration> _configurations;

        public IdentityDocumentConfigurationStore()
        {
            _configurations = Constants.StandardNetworkConfigurations;
        }

        public IQueryable<NetworkConfiguration> Query() => _configurations.AsQueryable();

        public void Add(NetworkConfiguration networkConfiguration) => _configurations.Add(networkConfiguration);

        public void Remove(NetworkConfiguration networkConfiguration) => _configurations.Remove(networkConfiguration);

        public Task<int> SaveChanges(CancellationToken cancellationToken) => Task.FromResult(1);
    }
}

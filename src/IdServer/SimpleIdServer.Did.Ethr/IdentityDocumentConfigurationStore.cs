// Copyright(c) SimpleIdServer.All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Ethr.Models;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Did.Ethr
{
    public interface IIdentityDocumentConfigurationStore
    {
        IQueryable<NetworkConfiguration> Query();
    }

    public class IdentityDocumentConfigurationStore : IIdentityDocumentConfigurationStore
    {
        private readonly ICollection<NetworkConfiguration> _configurations;

        public IdentityDocumentConfigurationStore()
        {
            _configurations = Constants.StandardNetworkConfigurations;
        }

        public IQueryable<NetworkConfiguration> Query() => _configurations.AsQueryable();
    }
}

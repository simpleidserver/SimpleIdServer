// Copyright(c) SimpleIdServer.All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Did
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
            _configurations = new List<NetworkConfiguration>
            {
                new NetworkConfiguration { Name = "aurora", RpcUrl = "https://mainnet.aurora.dev", ContractAdr = "0x63eD58B671EeD12Bc1652845ba5b2CDfBff198e0" }
            };
        }

        public IQueryable<NetworkConfiguration> Query() => _configurations.AsQueryable();
    }
}

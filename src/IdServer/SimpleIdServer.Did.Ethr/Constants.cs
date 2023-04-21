// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Ethr.Models;
using System.Collections.Generic;

namespace SimpleIdServer.Did.Ethr
{
    public static class Constants
    {
        public const string Type = "ethr";

        public const string DefaultContractAdr = "0x5721d5e733a2da7d805bdfb5177b4801cd86d3ae";

        public const string DefaultInfuraProjectId = "405e16111db4419e8d94431737f8ba53";

        public const string DefaultSource = "mainnet";

        public static ICollection<NetworkConfiguration> StandardNetworkConfigurations = new List<NetworkConfiguration>
        {
            new NetworkConfiguration { Name = "mainnet", RpcUrl = "https://mainnet.infura.io/v3/{0}", ContractAdr = DefaultContractAdr },
            new NetworkConfiguration { Name = "aurora", RpcUrl = "https://aurora-mainnet.infura.io/v3/{0}", ContractAdr = DefaultContractAdr },
            new NetworkConfiguration { Name = "sepolia", RpcUrl = "https://rpc.sepolia.org", ContractAdr = DefaultContractAdr }
        };
    }
}

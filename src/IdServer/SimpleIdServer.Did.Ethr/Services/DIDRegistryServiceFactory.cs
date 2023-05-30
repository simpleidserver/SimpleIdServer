// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Linq;

namespace SimpleIdServer.Did.Ethr.Services
{
    public interface IDIDRegistryServiceFactory
    {
        EthereumDIDRegistryService Build(string privateKey, string contractAdr, string network = Constants.DefaultSource);
    }

    public class DIDRegistryServiceFactory : IDIDRegistryServiceFactory
    {
        private readonly IIdentityDocumentConfigurationStore _configurationStore;
        private readonly DidEthrOptions _options;

        public DIDRegistryServiceFactory(IIdentityDocumentConfigurationStore configurationStore, IOptions<DidEthrOptions> options)
        {
            _configurationStore = configurationStore;
            _options = options.Value;
        }

        public EthereumDIDRegistryService Build(string privateKey, string contractAdr, string network = Constants.DefaultSource)
        {
            var account = new Account(privateKey.HexToByteArray());
            var networkConfiguration = _configurationStore.Query().Single(c => c.Name == network);
            var rpcUrl = string.Format(networkConfiguration.RpcUrl, _options.InfuraProjectId);
            var web3 = new Web3(account, rpcUrl);
            return new EthereumDIDRegistryService(web3, contractAdr);
        }
    }
}

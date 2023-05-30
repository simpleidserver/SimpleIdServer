// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using SimpleIdServer.Did.Ethr.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Did.Ethr.Services
{
    public class SmartContractService
    {
        private readonly IIdentityDocumentConfigurationStore _configurationStore;
        private readonly DidEthrOptions _options;
        private Account _account;
        private NetworkConfiguration _networkConfiguration;
        private Web3 _web3;

        public SmartContractService(IIdentityDocumentConfigurationStore configurationStore, IOptions<DidEthrOptions> options)
        {
            _configurationStore = configurationStore;
            _options = options.Value;
        }

        public SmartContractService UseAccount(string privateKey)
        {
            _account = new Account(privateKey.HexToByteArray());
            return this;
        }

        public SmartContractService UseNetwork(string network)
        {
            _networkConfiguration = _configurationStore.Query().Single(c => c.Name == network);
            var rpcUrl = string.Format(_networkConfiguration.RpcUrl, _options.InfuraProjectId);
            _web3 = new Web3(_account, rpcUrl);
            return this;
        }

        public Task<HexBigInteger> GetCurrentBalance() => _web3.Eth.GetBalance.SendRequestAsync(_account.Address);

        public Task<EthereumDIDRegistryService> DeployContractAndGetService()
        {
            var deployment = new EthereumDIDRegistryDeployment();
            var receipt = EthereumDIDRegistryService.DeployContractAndWaitForReceiptAsync(_web3, deployment).Result;
            var service = new EthereumDIDRegistryService(_web3, receipt.ContractAddress);
            return Task.FromResult(service);
        }
    }
}

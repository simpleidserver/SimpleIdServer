// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Ethr.Services;
using SimpleIdServer.Did.Models;
using System.Threading.Tasks;

namespace SimpleIdServer.Did.Ethr
{
    public class EthrContractDeploy : IContractDeploy
    {
        private readonly ISmartContractServiceFactory _smartContractServiceFactory;

        public EthrContractDeploy(ISmartContractServiceFactory smartContractServiceFactory)
        {
            _smartContractServiceFactory = smartContractServiceFactory;
        }

        public string Type => Constants.Type;

        public async Task<string> Deploy(NetworkConfiguration networkConfiguration)
        {
            var accountService = _smartContractServiceFactory.Build();
            var result = await accountService
                .UseAccount(networkConfiguration.PrivateAccountKey)
                .UseNetwork(networkConfiguration)
                .DeployContractAndGetService();
            return result.ContractHandler.ContractAddress;
        }
    }
}

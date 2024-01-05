// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using SimpleIdServer.Did.Ethr.Models;
using SimpleIdServer.Did.Ethr.Services;
using SimpleIdServer.Did.Ethr.Stores;
using SimpleIdServer.Did.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Did.Ethr;

public class DidEthrResolver : IDidResolver
{
    private readonly INetworkConfigurationStore _networkConfigurationStore;

    public DidEthrResolver(
        INetworkConfigurationStore networkConfigurationStore)
    {
        _networkConfigurationStore = networkConfigurationStore;
    }

    public string Method => Constants.Type;

    public async Task<DidDocument> Resolve(string did, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(did)) throw new ArgumentNullException(nameof(did));
        var decentralizedIdentifier = DidEthrExtractor.Extract(did);
        var networkConfiguration = await _networkConfigurationStore.Get(decentralizedIdentifier.Network, cancellationToken);
        if (networkConfiguration == null) throw new InvalidOperationException($"The network {decentralizedIdentifier.Network} is not valid");
        var service = new EthereumDIDRegistryService(
            new Web3(networkConfiguration.RpcUrl),
            networkConfiguration.ContractAdr);
        var w3 = new Web3(networkConfiguration.RpcUrl);
        var attributeChangedDTO = service.ContractHandler.GetEvent<DIDAttributeChangedEventDTO>();
        await ReadEvents(service, decentralizedIdentifier, networkConfiguration);
        return null;
    }

    private async Task<List<ERC1056Event>> ReadEvents(
        EthereumDIDRegistryService service,
        DecentralizedIdentifierEthr decentralizedIdentifier,
        NetworkConfiguration networkConfig)
    {
        var result = new List<ERC1056Event>();
        var changedQueryResult = await service.ChangedQueryAsync(decentralizedIdentifier.Address);
        var blockNumber = changedQueryResult;
        var logs = await service.ContractHandler.EthApiContractService.Filters.GetLogs.SendRequestAsync(new NewFilterInput
        {
            Address = new string[] { networkConfig.ContractAdr },
            FromBlock = new BlockParameter(new HexBigInteger(blockNumber)),
            ToBlock = new BlockParameter(new HexBigInteger(blockNumber))
        });
        return result;
    }
}

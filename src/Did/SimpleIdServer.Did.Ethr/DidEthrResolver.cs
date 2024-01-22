// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Nethereum.ABI.Decoders;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Encoders;
using SimpleIdServer.Did.Ethr.Models;
using SimpleIdServer.Did.Ethr.Services;
using SimpleIdServer.Did.Ethr.Stores;
using SimpleIdServer.Did.Extensions;
using SimpleIdServer.Did.Helpers;
using SimpleIdServer.Did.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
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
        var evts = await ReadEvents(service, decentralizedIdentifier, networkConfiguration);
        var builder = DidDocumentBuilder.New(did);
        var blockChainVerificationMethod = new DidDocumentVerificationMethod
        {
            Id = $"{did}#controller",
            Type = EcdsaSecp256k1RecoveryMethod2020Standard.TYPE,
            Controller = did,
            BlockChainAccountId = CAIP10BlockChainAccount.BuildEthereumMainet(decentralizedIdentifier.Address).ToString()
        };
        builder.AddVerificationMethod(blockChainVerificationMethod,
            VerificationMethodUsages.AUTHENTICATION | VerificationMethodUsages.ASSERTION_METHOD);
        if (decentralizedIdentifier.PublicKey != null)
        {
            var payload = decentralizedIdentifier.PublicKey.HexToByteArray();
            var publicKey = ES256KSignatureKey.From(payload, null);
            builder.AddVerificationMethod(EcdsaSecp256k1VerificationKey2019Standard.TYPE,
                publicKey,
                did,
                VerificationMethodUsages.AUTHENTICATION | VerificationMethodUsages.ASSERTION_METHOD,
                true,
                encoding: SignatureKeyEncodingTypes.HEX);
        }

        Consume(did, decentralizedIdentifier, builder, evts);
        return builder.Build();
    }

    private async Task<List<ERC1056Event>> ReadEvents(
        EthereumDIDRegistryService service,
        DecentralizedIdentifierEthr decentralizedIdentifier,
        NetworkConfiguration networkConfig)
    {
        var result = new List<ERC1056Event>();
        var attributeChangedDTO = service.ContractHandler.GetEvent<DIDAttributeChangedEventDTO>();
        var changedQueryResult = await service.ChangedQueryAsync(decentralizedIdentifier.Address);
        var blockNumber = changedQueryResult;
        var logs = await service.ContractHandler.EthApiContractService.Filters.GetLogs.SendRequestAsync(new NewFilterInput
        {
            Address = new string[] { networkConfig.ContractAdr },
            FromBlock = new BlockParameter(new HexBigInteger(blockNumber)),
            ToBlock = new BlockParameter(new HexBigInteger(blockNumber))
        });
        while(logs.Any())
        {
            var evt = Extract(logs, attributeChangedDTO);
            evt.BlockNumber = blockNumber;
            result.Add(evt);
            if (evt.PreviousChange < blockNumber)
            {
                blockNumber = evt.PreviousChange;
                logs = await service.ContractHandler.EthApiContractService.Filters.GetLogs.SendRequestAsync(new NewFilterInput
                {
                    Address = new string[] { networkConfig.ContractAdr },
                    FromBlock = new BlockParameter(new HexBigInteger(blockNumber)),
                    ToBlock = new BlockParameter(new HexBigInteger(blockNumber))
                });
            }
        }

        return result;
    }

    private ERC1056Event Extract(FilterLog[] logs, Event<DIDAttributeChangedEventDTO> attributeChangedDTO)
    {
        var attributeChanged = attributeChangedDTO.DecodeAllEventsForEvent(logs).First();
        var evt = attributeChanged.Event;
        var stringBytesDecoder = new StringBytes32Decoder();
        return new ERC1056Event
        {
            Identity = stringBytesDecoder.Decode(evt.Name),
            Value = evt.Value.ToHex(),
            PreviousChange = evt.PreviousChange,
            ValidTo = evt.ValidTo
        };
    }

    private void Consume(string id, DecentralizedIdentifierEthr did, DidDocumentBuilder builder, List<ERC1056Event> events)
    {
        /*
         * Only events with a validTo (measured in seconds) greater or equal to the current time should be included in the DID document. 
         * When resolving an older version (using versionId in the didURL query string), the validTo entry MUST be compared to the timestamp of the block of versionId height.
         */
        var now = (int)Math.Floor((decimal)DateTimeHelper.GetTime() / 1000);
        if (did.Version != null)
        {
            throw new NotImplementedException();
        }

        ConsumeDIDAttributeChanged(id, builder, events.Where(e => e.EventName == "DIDAttributeChanged").ToList(), now);
    }

    private void ConsumeDIDAttributeChanged(string id, DidDocumentBuilder builder, List<ERC1056Event> events, BigInteger now)
    {
        var regex = new Regex(@"^did\/(pub|svc)\/(\w+)(\/(\w+))?(\/(\w+))?$");
        int serviceCount = 0;
        int delegateCount = 0;
        var services = new Dictionary<string, DidDocumentService>();
        var verificationMethods = new Dictionary<string, DidDocumentVerificationMethod>();
        foreach (var evt in events.OrderByDescending(e => e.ValidTo))
        {
            if (!regex.IsMatch(evt.Identity)) continue;
            var eventIndex = $"{evt.Identity}-{evt.Value}";
            var splitted = evt.Identity.Split('/');
            if (evt.ValidTo  >= now)
            {
                var section = splitted.ElementAt(1);
                switch (section)
                {
                    case "pub":
                        delegateCount++;
                        var keyAlgorithm = splitted.ElementAt(2);
                        var keyPurpose = splitted.ElementAt(3);
                        var encoding = splitted.ElementAt(4);
                        var resolvedType = VerificationMethodTypeResolver.Resolve(keyAlgorithm);
                        if (string.IsNullOrWhiteSpace(resolvedType)) continue;
                        verificationMethods.Add(eventIndex, new DidDocumentVerificationMethod
                        {
                            Id = $"{id}#delegate-{delegateCount}",
                            Type = resolvedType,
                            Controller = id
                        });
                        // For all the formatters, check if the asymmetric key is correct.
                        // Have the possibility to retrieve the list of formatters for a given asymmetric key.
                        // All the formatters have common logic, such as encode into BASE58, or BASE64 or JWK or MULTICODEC.
                        // Read encoding...
                        // TODO : Refactor encoding.
                        break;
                    case "svc":
                        serviceCount++;
                        var serviceEndpoint = System.Text.Encoding.UTF8.GetString(Strip0X(evt.Value).HexToByteArray());
                        var type = splitted.ElementAt(2);
                        services.Add(eventIndex, new DidDocumentService
                        {
                            Id = $"{id}#service-{serviceCount}",
                            Type = type,
                            ServiceEndpoint = serviceEndpoint
                        });
                        break;
                }
            }
            else
            {
                var section = splitted.ElementAt(1);
                if (section == "svc") serviceCount++;
                if (section == "pub") delegateCount++;
                if (services.ContainsKey(eventIndex)) services.Remove(eventIndex);
                if (verificationMethods.ContainsKey(eventIndex)) verificationMethods.Remove(eventIndex);
            }
        }

        foreach (var kvp in services)
            builder.AddServiceEndpoint(kvp.Value);
    }

    private static string Strip0X(string str) => str.StartsWith("0x") ? new string(str.Skip(2).ToArray()) : str;
}

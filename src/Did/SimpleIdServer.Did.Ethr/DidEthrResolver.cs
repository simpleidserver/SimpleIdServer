// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Nethereum.ABI.Decoders;
using Nethereum.Contracts;
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
        did = decentralizedIdentifier.Format();
        var networkConfiguration = await _networkConfigurationStore.Get(decentralizedIdentifier.Network ?? Constants.DefaultNetwork, cancellationToken);
        if (networkConfiguration == null) throw new InvalidOperationException($"The network {decentralizedIdentifier.Network} is not valid");
        var service = new EthereumDIDRegistryService(
            new Web3(networkConfiguration.RpcUrl),
            networkConfiguration.ContractAdr);
        var now = (int)Math.Floor((decimal)DateTimeHelper.GetTime() / 1000);
        if (decentralizedIdentifier.Version != null)
        {
            var blockMetadata = await service.ContractHandler.EthApiContractService.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new BlockParameter(new Nethereum.Hex.HexTypes.HexBigInteger(decentralizedIdentifier.Version.Value)));
            now = (int)blockMetadata.Timestamp.Value;
        }

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
                encoding: SignatureKeyEncodingTypes.HEX,
                callback: c =>
                {
                    c.Id = $"{did}#controllerKey";
                });
        }

        Consume(did, decentralizedIdentifier, builder, evts, now, decentralizedIdentifier.Version);
        return builder.Build();
    }

    private async Task<List<ERC1056Event>> ReadEvents(
        EthereumDIDRegistryService service,
        DecentralizedIdentifierEthr decentralizedIdentifier,
        NetworkConfiguration networkConfig)
    {
        var result = new List<ERC1056Event>();
        var attributeChangedDTO = service.ContractHandler.GetEvent<DIDAttributeChangedEventDTO>();
        var ownerChangeDTO = service.ContractHandler.GetEvent<DIDOwnerChangedEventDTO>();
        var delegateChangedDTO = service.ContractHandler.GetEvent<DIDDelegateChangedEventDTO>();
        var changedQueryResult = await service.ChangedQueryAsync(decentralizedIdentifier.Address);
        var blockNumber = changedQueryResult;
        var logs = await service.ContractHandler.EthApiContractService.Filters.GetLogs.SendRequestAsync(new NewFilterInput
        {
            Address = new string[] { networkConfig.ContractAdr },
            FromBlock = new BlockParameter(new Nethereum.Hex.HexTypes.HexBigInteger(blockNumber)),
            ToBlock = new BlockParameter(new Nethereum.Hex.HexTypes.HexBigInteger(blockNumber))
        });
        while(logs.Any())
        {
            var evt = Extract(logs, attributeChangedDTO, ownerChangeDTO, delegateChangedDTO);
            if (evt == null) break;
            evt.BlockNumber = blockNumber;
            result.Add(evt);
            if (evt.PreviousChange < blockNumber)
            {
                blockNumber = evt.PreviousChange;
                logs = await service.ContractHandler.EthApiContractService.Filters.GetLogs.SendRequestAsync(new NewFilterInput
                {
                    Address = new string[] { networkConfig.ContractAdr },
                    FromBlock = new BlockParameter(new Nethereum.Hex.HexTypes.HexBigInteger(blockNumber)),
                    ToBlock = new BlockParameter(new Nethereum.Hex.HexTypes.HexBigInteger(blockNumber))
                });
            }
        }

        return result;
    }

    private ERC1056Event Extract(
        FilterLog[] logs,
        Event<DIDAttributeChangedEventDTO> attributeChangedDTO,
        Event<DIDOwnerChangedEventDTO> ownerChangedDTO,
        Event<DIDDelegateChangedEventDTO> delegateChangedDTO)
    {
        var evt = Extract(logs, attributeChangedDTO);
        if (evt == null)
        {
            evt = Extract(logs, ownerChangedDTO);
            if(evt == null)
            {
                return Extract(logs, delegateChangedDTO);
            }
        }

        return evt;
    }

    private ERC1056Event Extract(FilterLog[] logs, Event<DIDAttributeChangedEventDTO> attributeChangedDTO)
    {
        var attributeChangedLst = attributeChangedDTO.DecodeAllEventsForEvent(logs);
        if (!attributeChangedLst.Any()) return null;
        var evt = attributeChangedLst.First().Event;
        var stringBytesDecoder = new StringBytes32Decoder();
        return new ERC1056Event
        {
            Identity = stringBytesDecoder.Decode(evt.Name),
            Value = evt.Value.ToHex(),
            PreviousChange = evt.PreviousChange,
            ValidTo = evt.ValidTo,
            Type = EventTypes.DIDAttributeChanged
        };
    }

    private ERC1056Event Extract(FilterLog[] logs, Event<DIDOwnerChangedEventDTO> ownerChangedDTO)
    {
        var attributeChangedLst = ownerChangedDTO.DecodeAllEventsForEvent(logs);
        if (!attributeChangedLst.Any()) return null;
        var evt = attributeChangedLst.First().Event;
        return new ERC1056Event
        {
            Identity = evt.Identity,
            Owner = evt.Owner,
            PreviousChange = evt.PreviousChange,
            Type = EventTypes.DIDOwnerChanged
        };
    }

    private ERC1056Event Extract(FilterLog[] logs, Event<DIDDelegateChangedEventDTO> didDelegateChangedDTO)
    {
        var didDelegateChangeLst = didDelegateChangedDTO.DecodeAllEventsForEvent(logs);
        if (!didDelegateChangeLst.Any()) return null;
        var evt = didDelegateChangeLst.First().Event;
        var stringBytesDecoder = new StringBytes32Decoder();
        return new ERC1056Event
        {
            Identity = evt.Identity,
            Delegate = evt.Delegate,
            DelegateType = stringBytesDecoder.Decode(evt.DelegateType),
            PreviousChange = evt.PreviousChange,
            Type = EventTypes.DIDDelegateChanged
        };
    }

    private void Consume(string id, DecentralizedIdentifierEthr didEthr, DidDocumentBuilder builder, List<ERC1056Event> events, int now, int? blockNumber)
    {
        events.Reverse();
        ConsumeEvents(id, didEthr, builder, events, now, blockNumber);
    }

    private void ConsumeEvents(string id, DecentralizedIdentifierEthr didEthr, DidDocumentBuilder builder, List<ERC1056Event> events, BigInteger now, int? blockNumber)
    {
        var controller = id;
        var regex = new Regex(@"^did\/(pub|svc)\/(\w+)(\/(\w+))?(\/(\w+))?$");
        int serviceCount = 0;
        int delegateCount = 0;
        var authRefs = new Dictionary<string, string>();
        var sigRefs = new Dictionary<string, string>();
        var keyAgreementRefs = new Dictionary<string, string>();
        var services = new Dictionary<string, DidDocumentService>();
        var verificationMethods = new Dictionary<string, DidDocumentVerificationMethod>();
        foreach (var evt in events)
        {
            if(blockNumber != null && evt.BlockNumber > blockNumber.Value)
            {
                continue;
            }

            var eventName = Enum.GetName(typeof(EventTypes), evt.Type);
            var eventIndex = evt.Type == EventTypes.DIDDelegateChanged ? $"{eventName}-{evt.DelegateType}-{evt.Delegate}"
                : (evt.Type == EventTypes.DIDAttributeChanged ? $"{eventName}-{evt.Identity}-{evt.Value}" : eventName);
            var splitted = evt.Identity.Split('/');
            if (evt.ValidTo  >= now)
            {
                if(evt.Type == EventTypes.DIDDelegateChanged)
                {
                    delegateCount++;
                    var verificationMethodId = $"{id}#delegate-{delegateCount}";
                    switch(evt.DelegateType)
                    {
                        case "sigAuth":
                            authRefs.Add(eventIndex, verificationMethodId);
                            sigRefs.Add(eventIndex, verificationMethodId);
                            break;
                        case "veriKey":
                            var verificationMethod = new DidDocumentVerificationMethod
                            {
                                Id = verificationMethodId,
                                Type = EcdsaSecp256k1RecoveryMethod2020Standard.TYPE,
                                Controller = id,
                                BlockChainAccountId = CAIP10BlockChainAccount.BuildEthereumMainet(didEthr.Address).ToString()
                            };
                            verificationMethods.Add(eventIndex, verificationMethod);
                            sigRefs.Add(eventIndex, verificationMethod.Id);
                            break;
                    }
                }
                else if(evt.Type == EventTypes.DIDAttributeChanged)
                {
                    if (!regex.IsMatch(evt.Identity)) continue;
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
                            var verificationMethod = new DidDocumentVerificationMethod
                            {
                                Id = $"{id}#delegate-{delegateCount}",
                                Type = resolvedType,
                                Controller = controller
                            };
                            verificationMethods.Add(eventIndex, verificationMethod);
                            VerificationMethodUsages usage = VerificationMethodUsages.ASSERTION_METHOD;
                            if (keyPurpose == "sigAuth")
                            {
                                authRefs.Add(eventIndex, verificationMethod.Id);
                                sigRefs.Add(eventIndex, verificationMethod.Id);
                            }
                            else if (keyPurpose == "enc")
                            {
                                keyAgreementRefs.Add(eventIndex, verificationMethod.Id);
                            }
                            else
                            {
                                sigRefs.Add(eventIndex, verificationMethod.Id);
                            }

                            var payload = GetEventPayload(evt.Value);
                            switch (encoding)
                            {
                                case "hex":
                                    verificationMethod.PublicKeyHex = payload.ToHex();
                                    break;
                                case "base64":
                                    verificationMethod.PublicKeyBase64 = Convert.ToBase64String(payload);
                                    break;
                                case "base58":
                                    verificationMethod.PublicKeyBase58 = Encoding.Base58Encoding.Encode(payload);
                                    break;
                            }
                            break;
                        case "svc":
                            serviceCount++;
                            var serviceEndpoint = System.Text.Encoding.UTF8.GetString(GetEventPayload(evt.Value));
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
            }
            else if(evt.Type == EventTypes.DIDOwnerChanged)
            {
                controller = evt.Owner;
            }
            else
            {
                var section = splitted.ElementAt(1);
                if (section == "svc") serviceCount++;
                if (section == "pub") delegateCount++;
                if (authRefs.ContainsKey(eventIndex)) authRefs.Remove(eventIndex);
                if (sigRefs.ContainsKey(eventIndex)) sigRefs.Remove(eventIndex);
                if (services.ContainsKey(eventIndex)) services.Remove(eventIndex);
                if (verificationMethods.ContainsKey(eventIndex)) verificationMethods.Remove(eventIndex);
            }
        }

        foreach (var kvp in services)
            builder.AddServiceEndpoint(kvp.Value);

        foreach (var kvp in verificationMethods)
        {
            var usage = VerificationMethodUsages.ASSERTION_METHOD;
            var selectedKeyAgreement = keyAgreementRefs.FirstOrDefault(r => r.Value == kvp.Value.Id);
            var selectedAuthRef = authRefs.FirstOrDefault(r => r.Value == kvp.Value.Id);
            if (selectedKeyAgreement.Value != null) usage = VerificationMethodUsages.KEY_AGREEMENT;
            else if (selectedAuthRef.Value != null) usage |= VerificationMethodUsages.AUTHENTICATION;
            builder.AddVerificationMethod(kvp.Value, usage);
        }
    }

    private static byte[] GetEventPayload(string value)
    {
        value = value.StartsWith("0x") ? new string(value.Skip(2).ToArray()) : value;
        return value.HexToByteArray();
    }
}

// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Nethereum.ABI.Decoders;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using SimpleIdServer.Did.Ethr.Services;
using SimpleIdServer.Did.Models;
using SimpleIdServer.Did.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Did.Ethr
{
    public class IdentityDocumentExtractor : IIdentityDocumentExtractor
    {
        private readonly IIdentityDocumentConfigurationStore _store;
        private readonly DidEthrOptions _options;

        public IdentityDocumentExtractor(IIdentityDocumentConfigurationStore store, IOptions<DidEthrOptions> options)
        {
            _store = store;
            _options = options.Value;
        }

        public string Type => Constants.Type;

        public async Task<DidDocument> Extract(string id, CancellationToken cancellationToken)
        {
            var di = IdentityDocumentIdentifierParser.InternalParse(id);
            var networkConf = await _store.Get(di.Source, cancellationToken);
            if (networkConf == null) throw new InvalidOperationException($"the source {di.Source} is not supported");
            var rpcUrl = string.Format(networkConf.RpcUrl, _options.InfuraProjectId);
            var service = new EthereumDIDRegistryService(new Web3(networkConf.RpcUrl), networkConf.ContractAdr);
            var attributeChangedDTO = service.ContractHandler.GetEvent<DIDAttributeChangedEventDTO>();
            var evts = await ExtractEvents();
            return Build();

            async Task<ICollection<ERC1056Event>> ExtractEvents()
            {
                var result = new List<ERC1056Event>();
                var changedQueryResult = await service.ChangedQueryAsync(di.Address);
                var blockNumber = changedQueryResult;
                var logs = await service.ContractHandler.EthApiContractService.Filters.GetLogs.SendRequestAsync(new NewFilterInput
                {
                    Address = new string[] { networkConf.ContractAdr },
                    FromBlock = new BlockParameter(new HexBigInteger(blockNumber)),
                    ToBlock = new BlockParameter(new HexBigInteger(blockNumber))
                });
                while (logs.Any())
                {
                    var evt = Extract(logs);
                    evt.BlockNumber = blockNumber;
                    result.Add(evt);
                    if (evt.PreviousChange < blockNumber)
                    {
                        blockNumber = evt.PreviousChange;
                        logs = await service.ContractHandler.EthApiContractService.Filters.GetLogs.SendRequestAsync(new NewFilterInput
                        {
                            Address = new string[] { networkConf.ContractAdr },
                            FromBlock = new BlockParameter(new HexBigInteger(blockNumber)),
                            ToBlock = new BlockParameter(new HexBigInteger(blockNumber))
                        });
                    }
                }

                return result.OrderBy(r => r.BlockNumber).ToList();
            }

            ERC1056Event Extract(FilterLog[] logs)
            {
                var attributeChanged = attributeChangedDTO.DecodeAllEventsForEvent(logs).First();
                var evt = attributeChanged.Event;
                var tt = evt.ValidTo.ToHexBigInteger();
                var stringBytesDecoder = new StringBytes32Decoder();
                return new ERC1056Event
                {
                    Identity = stringBytesDecoder.Decode(evt.Name),
                    Value = evt.Value.ToHex(),
                    PreviousChange = evt.PreviousChange,
                    ValidTo = evt.ValidTo
                };
            }

            DidDocument Build()
            {
                var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                var diff = DateTime.UtcNow - origin;
                var controllerKey = di.Address;
                var now = new BigInteger(Math.Floor(diff.TotalSeconds));
                var result = new DidDocument
                {
                    // Context = new List<string> { "https://www.w3.org/ns/did/v1", "https://w3id.org/security/suites/secp256k1recovery-2020/v2" },
                    Id = id
                };
                int delegateCount = 0;
                int serviceCount = 0;
                var pks = new Dictionary<string, DidDocumentVerificationMethod>();
                var service = new Dictionary<string, DidDocumentService>();
                var auth = new Dictionary<string, string>();
                var assertion = new Dictionary<string, string>();
                var authentication = new List<string> { $"{id}#controller" };
                var keyAgreementRefs = new Dictionary<string, string>();
                var regex = new Regex(@"^did\/(pub|svc)\/(\w+)(\/(\w+))?(\/(\w+))?$");
                foreach (var evt in evts)
                {
                    var eventIndex = $"{evt.EventName}-{evt.Identity}-{evt.Value}";
                    // if (evt.ValidTo > now)
                    {
                        if (regex.IsMatch(evt.Identity))
                        {
                            var splitted = evt.Identity.Split('/');
                            var section = splitted.ElementAtOrDefault(1);
                            var algorithm = splitted.ElementAtOrDefault(2);
                            var type = splitted.ElementAtOrDefault(3);
                            if (type != null && Constants.LegacyAttrTypes.ContainsKey(type)) type = Constants.LegacyAttrTypes[type];
                            var encoding = splitted.ElementAtOrDefault(4);
                            switch(section)
                            {
                                case "pub":
                                    delegateCount++;
                                    var pk = new DidDocumentVerificationMethod
                                    {
                                        Id = $"{id}#delegate-{delegateCount}",
                                        Type = $"{algorithm}{type}",
                                        Controller = id
                                    };
                                    if (Constants.LegacyAlgos.ContainsKey(pk.Type)) pk.Type = Constants.LegacyAlgos[pk.Type];
                                    else pk.Type = algorithm;
                                    switch(encoding)
                                    {
                                        case "hex":
                                            pk.PublicKeyHex = Strip0X(evt.Value);
                                            break;
                                        case "base64":
                                            pk.PublicKeyBase64 = Convert.ToBase64String(Strip0X(evt.Value).HexToByteArray());
                                            break;
                                        default:
                                            pk.Value = Strip0X(evt.Value);
                                            break;
                                    }

                                    pks.Add(eventIndex, pk);
                                    if (splitted[3] == "sigAuth") auth.Add(eventIndex, pk.Id);
                                    else if (splitted[3] == "veriKey") assertion.Add(eventIndex, pk.Id);
                                    else if (splitted[3] == "enc") keyAgreementRefs.Add(eventIndex, pk.Id);
                                    break;
                                case "svc":
                                    serviceCount++;
                                    var endpoint = System.Text.Encoding.UTF8.GetString(Strip0X(evt.Value).HexToByteArray());
                                    service.Add(eventIndex, new DidDocumentService
                                    {
                                        Id = $"{id}#service-{serviceCount}",
                                        Type = algorithm,
                                        ServiceEndpoint = endpoint
                                    });
                                    break;
                            }
                        }
                    }
                }

                var publicKeys = new List<DidDocumentVerificationMethod>
                {
                    new DidDocumentVerificationMethod
                    {
                        Id = $"{id}#controller",
                        Type = Did.Constants.VerificationMethodTypes.EcdsaSecp256k1RecoveryMethod2020,
                        Controller = id,
                        BlockChainAccountId = $"eip155:1:{controllerKey}"
                    }
                };
                if(controllerKey == di.Address)
                {
                    publicKeys.Add(new DidDocumentVerificationMethod
                    {
                        Id = $"{id}#controllerKey",
                        Type = Did.Constants.VerificationMethodTypes.EcdsaSecp256k1VerificationKey2019,
                        Controller = id,
                        PublicKeyHex = Strip0X(controllerKey)
                    });
                    authentication.Add($"{id}#controllerKey");
                }

                publicKeys.AddRange(pks.Select(k => k.Value));
                foreach (var publicKey in publicKeys) result.AddVerificationMethod(publicKey);
                if (authentication.Any()) foreach (var a in authentication) result.AddAuthentication(a);
                if (service.Any()) foreach(var s in service.Select(s => s.Value).ToList()) result.AddService(s);
                if (assertion.Any()) foreach(var method in assertion.Select(kvp => kvp.Value)) result.AddAssertionMethod(method);
                return result;
            }

            string Strip0X(string str) => str.StartsWith("0x") ? new string(str.Skip(2).ToArray()) : str;
        }
    }
}

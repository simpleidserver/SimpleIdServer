// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Nethereum.ABI.Decoders;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using SimpleIdServer.Did.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Did
{
    public class IdentityDocumentExtractor
    {
        private static Dictionary<string, string> legacyAttrTypes = new Dictionary<string, string>
        {
            { "sigAuth", "SignatureAuthentication2018" },
            { "veriKey", "VerificationKey2018" },
            { "enc", "KeyAgreementKey2019" }
        };
        private static Dictionary<string, string> legacyAlgo = new Dictionary<string, string>
        {
            { "Secp256k1VerificationKey2018", VerificationMethodTypes.EcdsaSecp256k1VerificationKey2019 },
            { "Ed25519SignatureAuthentication2018", VerificationMethodTypes.Ed25519VerificationKey2018 },
            { "Secp256k1SignatureAuthentication2018", VerificationMethodTypes.EcdsaSecp256k1VerificationKey2019 },
            { "RSAVerificationKey2018", VerificationMethodTypes.RSAVerificationKey2018 },
            { "Ed25519VerificationKey2018", VerificationMethodTypes.Ed25519VerificationKey2018 },
            { "X25519KeyAgreementKey2019", VerificationMethodTypes.X25519KeyAgreementKey2019 }
        };
        private readonly IIdentityDocumentConfigurationStore _store;

        public IdentityDocumentExtractor(IIdentityDocumentConfigurationStore store)
        {
            _store = store;
        }

        public async Task<IdentityDocument> Extract(string id, CancellationToken cancellationToken)
        {
            var di = IdentityDocumentIdentifierParser.Parse(id);
            var networkConf = _store.Query().SingleOrDefault(n => n.Name == di.Source);
            if (networkConf == null) throw new InvalidOperationException($"the source {di.Source} is not supported");
            var service = new DIDService(new Web3(networkConf.RpcUrl), networkConf.ContractAdr);
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

            IdentityDocument Build()
            {
                var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                var diff = DateTime.UtcNow - origin;
                var now = new BigInteger(Math.Floor(diff.TotalSeconds));
                var result = new IdentityDocument
                {
                    Context = new List<string> { "https://www.w3.org/ns/did/v1", "https://w3id.org/security/suites/secp256k1recovery-2020/v2" },
                    Id = id
                };
                int delegateCount = 0;
                int serviceCount = 0;
                var pks = new Dictionary<string, IdentityDocumentVerificationMethod>();
                var auth = new Dictionary<string, string>();
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
                            var section = splitted[1];
                            var algorithm = splitted[2];
                            var type = splitted[3];
                            if (legacyAttrTypes.ContainsKey(type)) type = legacyAttrTypes[type];
                            var encoding = splitted[4];
                            switch(section)
                            {
                                case "pub":
                                    delegateCount++;
                                    var pk = new IdentityDocumentVerificationMethod
                                    {
                                        Id = $"{id}#delegate-{delegateCount}",
                                        Type = $"{algorithm}{type}",
                                        Controller = id
                                    };
                                    if (legacyAlgo.ContainsKey(pk.Type)) pk.Type = legacyAlgo[pk.Type];
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
                                    else if (splitted[3] == "enc") keyAgreementRefs.Add(eventIndex, pk.Id);
                                    break;
                                case "svc":

                            }

                            string ss = "";
                        }
                    }
                }
                return result;
            }

            string Strip0X(string str) => str.StartsWith("0x") ? str.Skip(2).ToList() : str;
        }
    }
}

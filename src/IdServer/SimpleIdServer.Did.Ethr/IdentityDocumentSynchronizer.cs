// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Nethereum.ABI.Encoders;
using Nethereum.Hex.HexConvertors.Extensions;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Ethr.Services;
using SimpleIdServer.Did.Events;
using SimpleIdServer.Did.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Did.Ethr
{
    public class IdentityDocumentSynchronizer
    {
        private readonly IDIDRegistryServiceFactory _factory;

        public IdentityDocumentSynchronizer(IDIDRegistryServiceFactory factory)
        {
            _factory = factory;
        }

        public async Task Sync(IdentityDocument document, string address, string privateKey, string contractAdr)
        {
            var parsedId = IdentityDocumentIdentifierParser.InternalParse(document.Id);
            var service = _factory.Build(privateKey, contractAdr, parsedId.Source);
            var changedAttributes = ExtractChangedAttributes(document, address);
            foreach(var changedAttribute in changedAttributes)
            {
                await service.SetAttributeRequestAsync(changedAttribute);
            }
        }

        private ICollection<SetAttributeFunction> ExtractChangedAttributes(IdentityDocument document, string address)
        {
            var result = new List<SetAttributeFunction>();
            var serviceAddedLst = document.Events.Where(e => e.Name == ServiceAdded.DEFAULT_NAME).Cast<ServiceAdded>();
            var verificationMethodAddedLst = document.Events.Where(e => e.Name == VerificationMethodAdded.DEFAULT_NAME).Cast<VerificationMethodAdded>().Select(v => v.VerificationMethod);
            var encoder = new Bytes32TypeEncoder();
            foreach (var serviceAdded in serviceAddedLst)
            {
                var hex = $"0x{System.Text.Encoding.UTF8.GetBytes(serviceAdded.ServiceEndpoint).ToHex()}";
                var value = hex.HexToByteArray();
                result.Add(new SetAttributeFunction
                {
                    Identity = address,
                    Name = encoder.Encode($"did/svc/{serviceAdded.Name}"),
                    Value = value
                });
            }

            foreach(var verificationMethod in verificationMethodAddedLst)
            {
                var type = document.GetKeyPurpose(verificationMethod);
                var alg = verificationMethod.Type;
                var publicKey = SignatureKeyFactory.ExtractPublicKey(verificationMethod);
                var hex = $"0x{publicKey.ToHex()}";
                var value = hex.HexToByteArray();
                var name = $"did/pub/{IdentityDocumentVerificationMethod.GetAlg(GetLegacyType(verificationMethod))}/{Constants.KeyPurposeNames[type]}/hex";
                result.Add(new SetAttributeFunction
                {
                    Identity = address,
                    Name = encoder.Encode(name),
                    Value = value
                });
            }

            return result;

            string GetLegacyType(IdentityDocumentVerificationMethod verificationMethod)
            {
                if (!Constants.LegacyAlgos.Values.Contains(verificationMethod.Type)) return verificationMethod.Type;
                return Constants.LegacyAlgos.First(kvp => kvp.Value == verificationMethod.Type).Key;
            }
        }
    }
}

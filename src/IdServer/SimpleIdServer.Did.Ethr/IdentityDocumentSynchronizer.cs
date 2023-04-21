// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Nethereum.ABI.Encoders;
using Nethereum.Hex.HexConvertors.Extensions;
using SimpleIdServer.Did.Ethr.Services;
using SimpleIdServer.Did.Events;
using SimpleIdServer.Did.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var encoder = new Bytes32TypeEncoder();
            foreach (var serviceAdded in serviceAddedLst)
            {
                var hex = $"0x{Encoding.UTF8.GetBytes(serviceAdded.ServiceEndpoint).ToHex()}";
                var value = hex.HexToByteArray();
                result.Add(new SetAttributeFunction
                {
                    Identity = address,
                    Name = encoder.Encode($"did/svc/{serviceAdded.Name}"),
                    Value = value
                });
            }

            return result;
        }
    }
}

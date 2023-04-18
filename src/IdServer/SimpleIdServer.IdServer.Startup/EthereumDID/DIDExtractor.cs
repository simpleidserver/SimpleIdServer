// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Nethereum.Web3;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Startup.EthereumDID
{
    public class DIDExtractor
    {
        public DIDExtractor()
        {

        }

        public void Extract(string id, DIDExtractionParameter parameter)
        {
            var service = new EthereumDIDService(new Web3(parameter.Url), contractAdr);
            // https://github.com/decentralized-identity/ethr-did-resolver/blob/master/src/resolver.ts
            var result = new DID
            {
                Context = new List<string> { "https://www.w3.org/ns/did/v1", "https://w3id.org/security/suites/secp256k1recovery-2020/v2" },
                Id = id
            };

        }
    }
}

// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using System;
using System.Linq;

namespace SimpleIdServer.Did
{
    public class IdentityDocumentIdentifierParser
    {
        public static IdentityDocumentIdentifier Parse(string id)
        {
            const string start = "did:ethr";
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
            if (!id.StartsWith(start)) throw new ArgumentException($"the DID Identifier must start by {start}");
            var splitted = id.Split(':');
            if (splitted.Count() != 3 && splitted.Count() != 4) throw new ArgumentException($"the DID Identifier must have the following format {start}:<identifier> or {start}:<source>:<identifier>");
            var result = new IdentityDocumentIdentifier(splitted.Last(), splitted.Count() == 4 ? splitted[2] : null);
            var payload = result.Identifier.HexToByteArray();
            if(result.Identifier.Count() > 42)
            {
                var publicKey = new EthECKey(payload, false);
                result.Address = publicKey.GetPublicAddress();
                result.PublicKey = publicKey;
            }
            else result.Address = result.Identifier;
            return result;
        }
    }
}

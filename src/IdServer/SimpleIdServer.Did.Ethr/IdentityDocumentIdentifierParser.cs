// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using SimpleIdServer.Did.Models;
using System;
using System.Linq;

namespace SimpleIdServer.Did.Ethr
{
    public class IdentityDocumentIdentifierParser : IIdentityDocumentIdentifierParser
    {
        public string Type => Constants.Type;

        public IdentityDocumentIdentifier Parse(string id) => InternalParse(id);

        internal static IdentityDocumentIdentifier InternalParse(string id)
        {
            const string start = "did:ethr";
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
            if (!id.StartsWith(start)) throw new ArgumentException($"the DID Identifier must start by {start}");
            var splitted = id.Split(':');
            if (splitted.Count() != 3 && splitted.Count() != 4) throw new ArgumentException($"the DID Identifier must have the following format {start}:<identifier> or {start}:<source>:<identifier>");
            var result = new IdentityDocumentIdentifier(splitted.Last(), splitted.Count() == 4 ? splitted[2] : Constants.DefaultSource);
            var payload = result.Identifier.HexToByteArray();
            if (result.Identifier.Count() > 42)
            {
                var publicKey = new EthECKey(payload, false);
                result.Address = publicKey.GetPublicAddress();
                result.PublicKey = result.Identifier.Replace("0x", string.Empty);
            }
            else result.Address = result.Identifier;
            return result;
        }
    }
}
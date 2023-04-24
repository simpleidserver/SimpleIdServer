// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Nethereum.Hex.HexConvertors.Extensions;
using SimpleIdServer.Did.Ethr;
using SimpleIdServer.Did.Models;

namespace SimpleIdServer.Did
{
    public class EthrIdentityDocumentBuilder : IdentityDocumentBuilder
    {
        protected EthrIdentityDocumentBuilder(IdentityDocument identityDocument) : base(identityDocument)
        {
        }

        public static EthrIdentityDocumentBuilder NewEthr(string did)
        {
            var parsed = IdentityDocumentIdentifierParser.InternalParse(did);
            var identityDocument = BuildDefaultDocument(did, parsed.Address);
            if(parsed.PublicKey != null)
            {
                var newId = $"{did}#controllerKey";
                identityDocument.AddVerificationMethod(new IdentityDocumentVerificationMethod
                {
                    Id = newId,
                    Type = VerificationMethodTypes.EcdsaSecp256k1VerificationKey2019,
                    Controller = did,
                    PublicKeyHex = parsed.PublicKey
                });
                identityDocument.AddAuthentication(newId);
                identityDocument.AddAssertionMethod(newId);
            }

            return new EthrIdentityDocumentBuilder(identityDocument);
        }
    }
}

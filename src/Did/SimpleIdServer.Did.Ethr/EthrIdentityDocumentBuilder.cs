// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Ethr;
using SimpleIdServer.Did.Models;

/*
namespace SimpleIdServer.Did
{
    public class EthrIdentityDocumentBuilder : DidDocumentBuilder
    {
        protected EthrIdentityDocumentBuilder(DidDocument identityDocument) : base(identityDocument)
        {
        }

        public static EthrIdentityDocumentBuilder NewEthr(string did)
        {
            var parsed = IdentityDocumentIdentifierParser.InternalParse(did);
            DidDocument identityDocument = null;
            // identityDocument.AddContext(Ethr.Constants.StandardSepc256K1RecoveryContext);
            identityDocument.AddVerificationMethod(new DidDocumentVerificationMethod
            {
                Id = $"{did}#controller",
                Type = Constants.VerificationMethodTypes.EcdsaSecp256k1RecoveryMethod2020,
                Controller = did,
                BlockChainAccountId = $"eip155:1:{parsed.Address}"
            });
            identityDocument.AddAuthentication($"{did}#controller");
            identityDocument.AddAssertionMethod($"{did}#controller");
            if (parsed.PublicKey != null)
            {
                var newId = $"{did}#controllerKey";
                identityDocument.AddVerificationMethod(new DidDocumentVerificationMethod
                {
                    Id = newId,
                    Type = Constants.VerificationMethodTypes.EcdsaSecp256k1VerificationKey2019,
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
*/
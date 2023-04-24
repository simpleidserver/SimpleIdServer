// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Models;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Did
{
    public class IdentityDocumentBuilder
    {
        private readonly IdentityDocument _identityDocument;

        protected IdentityDocumentBuilder(IdentityDocument identityDocument) 
        { 
            _identityDocument = identityDocument;
        }

        public static IdentityDocumentBuilder New(string did, string publicAdr) => new IdentityDocumentBuilder(BuildDefaultDocument(did, publicAdr));

        public static IdentityDocumentBuilder New(IdentityDocument identityDocument) => new IdentityDocumentBuilder(identityDocument);

        public IdentityDocumentBuilder AddServiceEndpoint(string type, string serviceEndpoint)
        {
            var id = $"{_identityDocument.Id}#service-{(_identityDocument.Service.Count() + 1)}";
            _identityDocument.AddService(new IdentityDocumentService
            {
                Id = id,
                Type = type,
                ServiceEndpoint = serviceEndpoint
            }, false);
            return this;
        }

        public IdentityDocumentBuilder AddVerificationMethod(ISignatureKey signatureKey, string type)
        {
            var verificationMethod = signatureKey.ExtractVerificationMethodWithPublicKey();
            var id = $"{_identityDocument.Id}#delegate-{(_identityDocument.VerificationMethod.Where(m => m.Id.Contains("#delegate")).Count() + 1)}";
            verificationMethod.Controller = _identityDocument.Id;
            verificationMethod.Id = _identityDocument.Id;
            verificationMethod.Type = type;
            _identityDocument.AddVerificationMethod(verificationMethod);
            _identityDocument.AddAssertionMethod(id);
            return this;
        }

        public IdentityDocument Build() => _identityDocument;

        protected static IdentityDocument BuildDefaultDocument(string did, string publicAdr)
        {
            var result = new IdentityDocument
            {
                Id = did,
                Context = new List<string>
                {
                    "https://www.w3.org/ns/did/v1", "https://w3id.org/security/suites/secp256k1recovery-2020/v2"
                }
            };
            result.AddVerificationMethod(new IdentityDocumentVerificationMethod
            {
                Id = $"{did}#controller",
                Type = VerificationMethodTypes.EcdsaSecp256k1RecoveryMethod2020,
                Controller = did,
                BlockChainAccountId = $"eip155:1:{publicAdr}"
            });
            result.AddAuthentication($"{did}#controller");
            result.AddAssertionMethod($"{did}#controller");
            return result;
        }
    }
}
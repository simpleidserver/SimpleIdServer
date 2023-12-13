// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Models;
using System;
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

        protected IdentityDocument IdentityDocument => _identityDocument;

        public static IdentityDocumentBuilder New(string did, string publicAdr) => new IdentityDocumentBuilder(BuildDefaultDocument(did));

        public static IdentityDocumentBuilder New(IdentityDocument identityDocument) => new IdentityDocumentBuilder(identityDocument);

        public IdentityDocumentBuilder AddContext(params string[] contextLst)
        {

            return this;
        }

        public IdentityDocumentBuilder AddContext(Action<ContextBuilder> callback)
        {
            return this;
        }

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

        public IdentityDocumentBuilder AddVerificationMethod(ISignatureKey signatureKey, string publicKeyFormat, KeyPurposes purpose = KeyPurposes.VerificationKey)
        {
            var verificationMethod = signatureKey.ExtractVerificationMethodWithPublicKey();
            var id = $"{_identityDocument.Id}#delegate-{(_identityDocument.VerificationMethod.Where(m => m.Id.Contains("#delegate")).Count() + 1)}";
            verificationMethod.Controller = _identityDocument.Id;
            verificationMethod.Id = id;
            verificationMethod.Type = publicKeyFormat;
            _identityDocument.AddVerificationMethod(verificationMethod, false);
            return AddVerificationMethod(verificationMethod, purpose);
        }

        public IdentityDocumentBuilder AddVerificationMethod(IdentityDocumentVerificationMethod verificationMethod, KeyPurposes purpose = KeyPurposes.VerificationKey)
        {
            switch (purpose)
            {
                case KeyPurposes.VerificationKey:
                    _identityDocument.AddAssertionMethod(verificationMethod.Id);
                    break;
                case KeyPurposes.SigAuthentication:
                    _identityDocument.AddAuthentication(verificationMethod.Id);
                    break;
                default:
                    throw new InvalidOperationException("enc is not supported");
            }

            return this;
        }

        public IdentityDocument Build()
        {
            // Always add https://www.w3.org/ns/did/v1 in the @context
            return _identityDocument;
        }

        protected static IdentityDocument BuildDefaultDocument(string did)
        {
            var result = new IdentityDocument
            {
                Id = did,
                Context = new List<string>
                {
                    Constants.DefaultIdentityDocumentContext
                }
            };
            return result;
        }
    }
}
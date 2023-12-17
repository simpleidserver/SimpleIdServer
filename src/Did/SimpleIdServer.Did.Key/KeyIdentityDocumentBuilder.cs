// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Models;

namespace SimpleIdServer.Did.Key
{
    public class KeyIdentityDocumentBuilder : IdentityDocumentBuilder
    {
        protected KeyIdentityDocumentBuilder(IdentityDocument identityDocument) : base(identityDocument)
        {
        }

        public static KeyIdentityDocumentBuilder NewKey(string did)
        {
            var parsed = IdentityDocumentIdentifierParser.InternalParse(did);
            IdentityDocument identityDocument = null;
            return new KeyIdentityDocumentBuilder(identityDocument);
        }

        public static KeyIdentityDocumentBuilder NewKey(ISignatureKey signatureKey) => NewKey(IdentityDocumentIdentifierBuilder.Build(signatureKey));

        public new KeyIdentityDocumentBuilder AddVerificationMethod(ISignatureKey signatureKey, string publicKeyFormat = Did.Constants.VerificationMethodTypes.JsonWebKey2020, KeyPurposes purpose = KeyPurposes.VerificationKey)
        {
            var publicKeyMultibase = IdentityDocumentIdentifierBuilder.GetPublicKeyBase(signatureKey);
            var verificationMethodId = $"{IdentityDocument.Id}#{publicKeyMultibase}";
            var identityVerificationMethod = new IdentityDocumentVerificationMethod
            {
                Id = verificationMethodId,
                Controller = IdentityDocument.Id,
                Type = publicKeyFormat
            };
            if (publicKeyFormat == Did.Constants.VerificationMethodTypes.Ed25519VerificationKey2020)
                identityVerificationMethod.AdditionalParameters.Add(Constants.AdditionalVerificationMethodFields.PublicKeyMultibase, publicKeyMultibase);

            if (publicKeyFormat == Did.Constants.VerificationMethodTypes.JsonWebKey2020)
                identityVerificationMethod.PublicKeyJwk = signatureKey.GetPublicKeyJwk();

            IdentityDocument.VerificationMethod.Add(identityVerificationMethod);
            // AddVerificationMethod(identityVerificationMethod, purpose);
            return this;
        }
    }
}

// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Models;
using System;
using System.Linq;

namespace SimpleIdServer.Did.Jwt
{
    public static class DidJwtValidator
    {
        public static bool Validate(string json, DidDocument document, KeyPurposes keyPurpose = KeyPurposes.SigAuthentication)
        {
            var handler = new JsonWebTokenHandler();
            var jwt = handler.ReadJsonWebToken(json);
            return Validate(jwt, document, keyPurpose);
        }

        public static bool Validate(JsonWebToken jwt, DidDocument document, KeyPurposes keyPurpose = KeyPurposes.SigAuthentication)
        {
            var alg = jwt.Alg;
            if (!Constants.SupportedPublicKeyTypes.ContainsKey(alg)) throw new InvalidOperationException($"Alg {alg} is not supported");
            var keyTypes = Constants.SupportedPublicKeyTypes[alg];
            var ids = document.Authentication;
            // if (keyPurpose == KeyPurposes.VerificationKey) ids = document.AssertionMethod;
            if (!ids.Any()) throw new InvalidOperationException($"There is no key to used for the proof type {keyPurpose}");
            var authenticators = document.VerificationMethod.Where(m => keyTypes.Contains(m.Type));
            if (!authenticators.Any()) throw new InvalidOperationException($"DID Document does not have public keyh for {alg}");
            foreach (var authenticator in authenticators)
                if (Check(jwt, authenticator, alg)) return true;
            return false;
        }

        private static bool Check(JsonWebToken jwt, DidDocumentVerificationMethod method, string alg)
        {
            var content = $"{jwt.EncodedHeader}.{jwt.EncodedPayload}";
            var signature = jwt.EncodedSignature;
            var sigKey = SignatureKeyFactory.Build(method, alg);
            if (sigKey == null) return false;
            var result = sigKey.Check(content, signature);
            return result;
        }
    }
}
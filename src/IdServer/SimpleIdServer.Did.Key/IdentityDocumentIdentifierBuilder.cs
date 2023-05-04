// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Crypto;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Did.Key
{
    public static class IdentityDocumentIdentifierBuilder
    {
        public static string Build(ISignatureKey signatureKey) => $"{Constants.Namespace}:{GetPublicKeyBase(signatureKey)}";

        public static string GetPublicKeyBase(ISignatureKey signatureKey)
        {
            var kvp = SignatureKeyFactory.AlgToMulticodec.First(kvp => kvp.Value == signatureKey.Name);
            var publicKeyPayload = signatureKey.GetPublicKey(true);
            var payload = new List<byte>();
            payload.AddRange(kvp.Key.Item2);
            payload.AddRange(publicKeyPayload);
            var publicKeyMultibase = $"z{Encoding.Base58Encoding.Encode(payload.ToArray())}";
            return publicKeyMultibase;
        }
    }
}

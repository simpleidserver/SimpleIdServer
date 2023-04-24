// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.Did
{
    public static class SupportedPublicKeyTypes
    {
        public static Dictionary<string, List<string>> Values => new Dictionary<string, List<string>>
        {
            {
                SupportedSignatureKeyAlgs.ES256K, new List<string>
                {
                    VerificationMethodTypes.EcdsaSecp256k1VerificationKey2019,
                    VerificationMethodTypes.EcdsaSecp256k1RecoveryMethod2020,
                    VerificationMethodTypes.Secp256k1VerificationKey2018,
                    VerificationMethodTypes.Secp256k1SignatureVerificationKey2018,
                    VerificationMethodTypes.EcdsaPublicKeySecp256k1
                }
            },
            {
                SupportedSignatureKeyAlgs.Ed25519, new List<string>
                {
                    VerificationMethodTypes.ED25519SignatureVerification,
                    VerificationMethodTypes.Ed25519VerificationKey2018,
                    VerificationMethodTypes.Ed25519VerificationKey2020,
                    VerificationMethodTypes.JsonWebKey2020
                }
            },
            {
                SupportedSignatureKeyAlgs.ES256, new List<string>
                {
                    VerificationMethodTypes.JsonWebKey2020
                }
            }
        };
    }
}

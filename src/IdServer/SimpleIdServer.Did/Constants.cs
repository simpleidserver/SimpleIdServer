// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.Did
{
    public static class Constants
    {
        public static Dictionary<string, List<string>> SupportedPublicKeyTypes = new Dictionary<string, List<string>>
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

        public static class LegacyAttributeTypes
        {
            public static string SignatureAuthentication2018 = "SignatureAuthentication2018";
            public static string VerificationKey2018 = "VerificationKey2018";
            public static string VerificationKey2019 = "VerificationKey2019";
            public static string VerificationKey2020 = "VerificationKey2020";
            public static string KeyAgreementKey2019 = "KeyAgreementKey2019";
            public static string SignatureVerification = "SignatureVerification";
        }

        public static class VerificationMethodTypes
        {
            public static string Ed25519VerificationKey2018 = $"Ed25519{LegacyAttributeTypes.VerificationKey2018}";
            public static string RSAVerificationKey2018 = $"RSA{LegacyAttributeTypes.VerificationKey2018}";
            public static string Secp256k1VerificationKey2018 = $"Secp256k1{LegacyAttributeTypes.VerificationKey2018}";
            public static string Secp256k1SignatureVerificationKey2018 = $"Secp256k1Signature{LegacyAttributeTypes.VerificationKey2018}";
            public static string EcdsaSecp256k1VerificationKey2019 = $"EcdsaSecp256k1{LegacyAttributeTypes.VerificationKey2019}";
            public static string Ed25519VerificationKey2020 = $"Ed25519{LegacyAttributeTypes.VerificationKey2020}";
            public const string EcdsaSecp256k1RecoveryMethod2020 = "EcdsaSecp256k1RecoveryMethod2020";
            public static string ED25519SignatureVerification = $"ED25519{LegacyAttributeTypes.SignatureVerification}";
            public static string X25519KeyAgreementKey2019 = $"X25519{LegacyAttributeTypes.KeyAgreementKey2019}";
            public const string EcdsaPublicKeySecp256k1 = "EcdsaPublicKeySecp256k1";
            public const string JsonWebKey2020 = "JsonWebKey2020";
        }

        public class SupportedSignatureKeyAlgs
        {
            public const string ES256K = "ES256K";
            public const string ES256 = "ES256";
            public const string Ed25519 = "Ed25519";
        }
    }
}

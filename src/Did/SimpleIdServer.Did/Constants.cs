// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.Did;

public static class Constants
{
    public static class StandardKty
    {
        /// <summary>
        /// Octet string key pair.
        /// </summary>
        public const string OKP = "OKP";
        /// <summary>
        /// Elliptic curve.
        /// </summary>
        public const string EC = "EC";
        /// <summary>
        /// RSA
        /// </summary>
        public const string RSA = "RSA";
    }

    public static class StandardCrvOrSize
    {
        /// <summary>
        /// Ed25519 signature algorithm key pairs
        /// </summary>
        public const string Ed25519 = "Ed25519";
        /// <summary>
        /// X25519 function key pairs
        /// </summary>
        public const string X25519 = "X25519";
        /// <summary>
        /// SECG secp256k1 curve
        /// </summary>
        public const string SECP256k1 = "secp256k1";
        /// <summary>
        /// P-256 Curve
        /// </summary>
        public const string P256 = "P-256";
        /// <summary>
        /// P-384 Curve
        /// </summary>
        public const string P384 = "P-384";
        public const string RSA2048 = "2048+";
    }

    public static class StandardJwtAlgs
    {
        public const string Es256K = "ES256K";
        public const string EdDsa = "EdDSA";
        public const string Es256 = "ES256";
        public const string Es384 = "ES384";
        public const string Rsa = "RS256";
        public const string X25519 = "X25519";
    }

    public const string DefaultIdentityDocumentContext = "https://www.w3.org/ns/did/v1";
    public const string Scheme = "did";
}

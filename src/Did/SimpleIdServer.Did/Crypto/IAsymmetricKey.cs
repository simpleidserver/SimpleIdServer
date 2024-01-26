// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace SimpleIdServer.Did.Crypto;

public interface IAsymmetricKey
{
    string Kty { get; }
    string CrvOrSize { get; }
    void Import(byte[] publicKey, byte[] privateKey);
    void Import(JsonWebKey publicKey, JsonWebKey privateKey);
    byte[] GetPublicKey(bool compressed = false);
    byte[] GetPrivateKey();
    JsonWebKey GetPublicJwk();
    JsonWebKey GetPrivateJwk();
    byte[] SignHash(byte[] content, HashAlgorithmName alg);
    bool CheckHash(byte[] content, byte[] signature, HashAlgorithmName? alg = null);
    SigningCredentials BuildSigningCredentials();
}

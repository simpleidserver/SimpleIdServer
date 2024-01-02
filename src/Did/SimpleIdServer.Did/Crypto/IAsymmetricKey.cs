// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;

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
    byte[] Sign(byte[] content);
    bool Check(byte[] content, byte[] signature);
}

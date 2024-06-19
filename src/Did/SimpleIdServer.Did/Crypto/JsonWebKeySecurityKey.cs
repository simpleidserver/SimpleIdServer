// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace SimpleIdServer.Did.Crypto;

public class JsonWebKeySecurityKey : IAsymmetricKey
{
    private readonly JsonWebKey _jwk;

    public JsonWebKeySecurityKey(JsonWebKey jwk)
    {
        _jwk = jwk;
    }

    public string Kty => string.Empty;

    public string CrvOrSize => string.Empty;

    public string JwtAlg => string.Empty;

    public SigningCredentials BuildSigningCredentials(string kid = null)
    {
        return new SigningCredentials(_jwk, _jwk.Alg);
    }

    public bool CheckHash(byte[] content, byte[] signature, HashAlgorithmName? alg = null)
    {
        throw new System.NotImplementedException();
    }

    public JsonWebKey GetPrivateJwk()
    {
        throw new System.NotImplementedException();
    }

    public byte[] GetPrivateKey()
    {
        throw new System.NotImplementedException();
    }

    public JsonWebKey GetPublicJwk()
        => _jwk;

    public byte[] GetPublicKey(bool compressed = false)
    {
        throw new System.NotImplementedException();
    }

    public void Import(byte[] publicKey, byte[] privateKey)
    {
        throw new System.NotImplementedException();
    }

    public void Import(JsonWebKey publicKey, JsonWebKey privateKey)
    {
        throw new System.NotImplementedException();
    }

    public byte[] SignHash(byte[] content, HashAlgorithmName alg)
    {
        throw new System.NotImplementedException();
    }
}

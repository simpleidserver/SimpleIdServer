// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Did.Serializers;
using System;
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
        if (_jwk == null) throw new InvalidOperationException("there is no private key");
        _jwk.Kid = kid;
        return new SigningCredentials(_jwk, _jwk.Alg);
    }

    public bool CheckHash(byte[] content, byte[] signature, HashAlgorithmName? alg = null)
    {
        throw new System.NotImplementedException();
    }

    public JsonWebKey GetPrivateJwk()
        => _jwk;

    public byte[] GetPrivateKey()
        => GetPublicKey();

    public JsonWebKey GetPublicJwk()
        => _jwk;

    public byte[] GetPublicKey(bool compressed = false)
    {
        if (_jwk == null) throw new InvalidOperationException("there is no public key");
        _jwk.Alg = null;
        var json = JsonWebKeySerializer.Write(_jwk);
        return System.Text.Encoding.UTF8.GetBytes(json);
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

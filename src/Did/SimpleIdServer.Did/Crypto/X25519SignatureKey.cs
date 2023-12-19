// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using System;

namespace SimpleIdServer.Did.Crypto;

/// <summary>
///  About agreement. https://github.com/bcgit/bc-csharp/blob/e9cfe0211e25f9fc300329963050e96ef6634b08/crypto/test/src/crypto/test/X25519Test.cs#L42
/// </summary>
public class X25519SignatureKey : ISignatureKey
{
    public string Name => "X25519KeyAgreementKey2019";

    public byte[] PrivateKey => throw new System.NotImplementedException();

    public void Import(byte[] publicKey, byte[] privateKey)
    {
        throw new NotImplementedException();
    }

    public void Import(JsonWebKey jwk)
    {
        throw new NotImplementedException();
    }

    public bool Check(string content, string signature)
    {
        throw new System.NotImplementedException();
    }

    public bool Check(byte[] content, byte[] signature)
    {
        throw new System.NotImplementedException();
    }

    public string Sign(string content)
    {
        throw new System.NotImplementedException();
    }

    public string Sign(byte[] content)
    {
        throw new System.NotImplementedException();
    }

    public byte[] GetPublicKey(bool compressed = false)
    {
        throw new System.NotImplementedException();
    }

    public JsonWebKey GetPublicJwk()
    {
        throw new System.NotImplementedException();
    }
}

// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;

namespace SimpleIdServer.Did.Crypto;

public class X25519SignatureKey : ISignatureKey
{
    public string Name => throw new System.NotImplementedException();

    public byte[] PrivateKey => throw new System.NotImplementedException();

    public bool Check(string content, string signature)
    {
        throw new System.NotImplementedException();
    }

    public bool Check(byte[] content, byte[] signature)
    {
        throw new System.NotImplementedException();
    }

    public byte[] GetPublicKey(bool compressed = false)
    {
        throw new System.NotImplementedException();
    }

    public JsonWebKey GetPublicKeyJwk()
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
}

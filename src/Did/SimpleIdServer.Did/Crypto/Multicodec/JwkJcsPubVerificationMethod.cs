// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;

namespace SimpleIdServer.Did.Crypto.Multicodec;

public class JwkJcsPubVerificationMethod : IVerificationMethod
{
    public const string MULTICODES_HEX_VALUE = "0xd1d603";

    public string MulticodecPublicKeyHexValue => MULTICODES_HEX_VALUE;

    public string MulticodecPrivateKeyHexValue => null;

    public int KeySize => 0;

    public string Kty => null;

    public string CrvOrSize => null;

    public IAsymmetricKey Build(byte[] publicKey, byte[] privateKey)
    {
        var json = System.Text.Encoding.UTF8.GetString(publicKey);
        var jsonWebToken = new JsonWebKey(json);
        return new JsonWebKeySecurityKey(jsonWebToken);
    }

    public IAsymmetricKey Build(JsonWebKey publicJwk, JsonWebKey privateJwk)
    {
        return null;
    }
}
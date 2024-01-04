// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;

namespace SimpleIdServer.Did.Crypto.Multicodec;

public class RSAVerificationMethod : IVerificationMethod
{
    public string MulticodecPublicKeyHexValue => MULTICODEC_HEX_PUBLIC_VALUE;

    public string MulticodecPrivateKeyHexValue => MULTICODEC_HEX_VALUE_PRIVATE_KEY;

    public const string MULTICODEC_HEX_PUBLIC_VALUE = "0x1205";

    public const string MULTICODEC_HEX_VALUE_PRIVATE_KEY = "0x1305";

    public int KeySize => 2048;

    public string Kty => Constants.StandardKty.RSA;

    public string CrvOrSize => Constants.StandardCrvOrSize.RSA2048;

    public IAsymmetricKey Build(byte[] publicKey, byte[] privateKey)
        => RSASignatureKey.From(publicKey, privateKey);

    public IAsymmetricKey Build(JsonWebKey publicJwk, JsonWebKey privateJwk)
        => RSASignatureKey.From(publicJwk, privateJwk);
}
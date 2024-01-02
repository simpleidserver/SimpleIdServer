// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;

namespace SimpleIdServer.Did.Crypto.Multicodec;

public class Es384VerificationMethod : IVerificationMethod
{
    public string MulticodecPublicKeyHexValue => MULTICODEC_HEX_VALUE;

    public string MulticodecPrivateKeyHexValue => MULTICODEC_HEX_VALUE_PRIVATE_KEY;


    public const string MULTICODEC_HEX_VALUE = "0x1201";

    public const string MULTICODEC_HEX_VALUE_PRIVATE_KEY = "0x1307";

    public int KeySize => 49;

    public string Kty => Constants.StandardKty.EC;

    public string CrvOrSize => Constants.StandardCrvOrSize.P384;

    public IAsymmetricKey Build(byte[] publicKey, byte[] privateKey)
        => ES384SignatureKey.From(publicKey, privateKey);

    public IAsymmetricKey Build(JsonWebKey publicJwk, JsonWebKey privateJwk)
        => ES384SignatureKey.From(publicJwk, privateJwk);
}

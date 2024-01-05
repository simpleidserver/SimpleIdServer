// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;

namespace SimpleIdServer.Did.Crypto.Multicodec;

public class X25519VerificationMethod : IVerificationMethod
{
    public string MulticodecPublicKeyHexValue => MULTICODEC_HEX_VALUE;

    public string MulticodecPrivateKeyHexValue => MULTICODEC_HEX_VALUE_PRIVATE_KEY;


    public const string MULTICODEC_HEX_VALUE = "0xec01";

    public const string MULTICODEC_HEX_VALUE_PRIVATE_KEY = "0x1302";

    public int KeySize => 32;

    public string Kty => Constants.StandardKty.OKP;

    public string CrvOrSize => Constants.StandardCrvOrSize.X25519;

    public IAsymmetricKey Build(byte[] publicKey, byte[] privateKey)
        => X25519AgreementKey.From(publicKey, privateKey);

    public IAsymmetricKey Build(JsonWebKey publicJwk, JsonWebKey privateJwk)
        => X25519AgreementKey.From(publicJwk, privateJwk);
}

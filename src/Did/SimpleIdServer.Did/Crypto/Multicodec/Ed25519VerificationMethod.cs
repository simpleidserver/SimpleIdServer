// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;

namespace SimpleIdServer.Did.Crypto.Multicodec;

public class Ed25519VerificationMethod : IVerificationMethod
{
    public string MulticodecPublicKeyHexValue => MULTICODEC_HEX_PUBLIC_VALUE;

    public string MulticodecPrivateKeyHexValue => MULTICODEC_HEX_VALUE_PRIVATE_KEY;

    public const string MULTICODEC_HEX_PUBLIC_VALUE = "0xed01";

    public const string MULTICODEC_HEX_VALUE_PRIVATE_KEY = "0x8026";

    public string Kty => Constants.StandardKty.OKP;

    public string CrvOrSize => Constants.StandardCrvOrSize.Ed25519;

    public int KeySize => 32;

    public IAsymmetricKey Build(byte[] payload) 
        => Ed25519SignatureKey.From(payload, null);

    public IAsymmetricKey Build(JsonWebKey jwk)
        => Ed25519SignatureKey.From(jwk);
}
// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;

namespace SimpleIdServer.Did.Crypto.Multicodec;

public class Es256KVerificationMethod : IVerificationMethod
{
    public string MulticodecPublicKeyHexValue => MULTICODES_HEX_VALUE;

    public string MulticodecPrivateKeyHexValue => MULTICODEC_HEX_VALUE_PRIVATE_KEY;

    public const string MULTICODES_HEX_VALUE = "0xe7";

    public const string MULTICODEC_HEX_VALUE_PRIVATE_KEY = "0x1301";

    public string Kty => Constants.StandardKty.EC;

    public string CrvOrSize => Constants.StandardCrvOrSize.SECP256k1;

    public int KeySize => 33;

    public IAsymmetricKey Build(byte[] publicKey, byte[] privateKey) 
        => ES256KSignatureKey.From(publicKey, privateKey);

    public IAsymmetricKey Build(JsonWebKey publicJwk, JsonWebKey privateJwk)
        => ES256KSignatureKey.From(publicJwk, privateJwk);
}

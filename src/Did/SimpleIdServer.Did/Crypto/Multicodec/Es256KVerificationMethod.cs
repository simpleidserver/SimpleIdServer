// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;

namespace SimpleIdServer.Did.Crypto.Multicodec;

public class Es256KVerificationMethod : IVerificationMethod
{
    public string MulticodecPublicKeyHexValue => MULTICODES_HEX_VALUE;

    public string MulticodecPrivateKeyHexValue => string.Empty;

    public const string MULTICODES_HEX_VALUE = "0xe7";

    public string Kty => Constants.StandardKty.EC;

    public string CrvOrSize => Constants.StandardCrvOrSize.SECP256k1;

    public int KeySize => 33;

    public IAsymmetricKey Build(byte[] payload) 
        => ES256KSignatureKey.From(payload, null);

    public IAsymmetricKey Build(JsonWebKey jwk)
        => ES256KSignatureKey.From(jwk);
}

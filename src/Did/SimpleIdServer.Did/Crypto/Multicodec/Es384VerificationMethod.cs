// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;

namespace SimpleIdServer.Did.Crypto.Multicodec;

public class Es384VerificationMethod : IVerificationMethod
{
    public string MulticodecHexValue => MULTICODES_HEX_VALUE;

    public const string MULTICODES_HEX_VALUE = "0x1201";

    public int KeySize => 49;

    public string Kty => Constants.StandardKty.EC;

    public string CrvOrSize => Constants.StandardCrvOrSize.P384;

    public IAsymmetricKey Build(byte[] payload)
        => ES384SignatureKey.From(payload, null);

    public IAsymmetricKey Build(JsonWebKey jwk)
        => ES384SignatureKey.From(jwk);
}

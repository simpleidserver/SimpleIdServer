// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;

namespace SimpleIdServer.Did.Crypto.Multicodec;

public class X25519VerificationMethod : IVerificationMethod
{
    public string MulticodecPublicKeyHexValue => MULTICODEC_HEX_VALUE;

    public string MulticodecPrivateKeyHexValue => string.Empty;


    public const string MULTICODEC_HEX_VALUE = "0xec";

    public int KeySize => 32;

    public string Kty => Constants.StandardKty.OKP;

    public string CrvOrSize => Constants.StandardCrvOrSize.X25519;

    public IAsymmetricKey Build(byte[] payload)
        => X25519AgreementKey.From(payload, null);

    public IAsymmetricKey Build(JsonWebKey jwk)
        => X25519AgreementKey.From(jwk);
}

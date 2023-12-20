// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Sec;

namespace SimpleIdServer.Did.Crypto;

public class ES384SignatureKey : BaseESSignatureKey
{
    private ES384SignatureKey() : base(SecNamedCurves.GetByName("secp384r1"))
    {
    }

    public override string Name => Constants.SupportedSignatureKeyAlgs.ES384;

    protected override string CurveName => "P-384";

    public static ES384SignatureKey Generate()
    {
        var result = new ES384SignatureKey();
        result.Random();
        return result;
    }

    public static ES384SignatureKey From(byte[] publicKey, byte[] privateKey)
    {
        var result = new ES384SignatureKey();
        result.Import(publicKey, privateKey);
        return result;
    }

    public static ES384SignatureKey From(JsonWebKey jwk)
    {
        var result = new ES384SignatureKey();
        result.Import(jwk);
        return result;
    }
}

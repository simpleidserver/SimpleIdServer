// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Sec;

namespace SimpleIdServer.Did.Crypto;

/// <summary>
/// https://identity.foundation/context/did-latest.html
/// </summary>
public class ES256KSignatureKey : BaseESSignatureKey
{
    private ES256KSignatureKey() : base(SecNamedCurves.GetByName(Constants.StandardCrvOrSize.SECP256k1)) { }

    public override string CrvOrSize => Constants.StandardCrvOrSize.SECP256k1;

    public static ES256KSignatureKey Generate()
    {
        var result = new ES256KSignatureKey();
        result.Random();
        return result;
    }

    public static ES256KSignatureKey From(byte[] publicKey, byte[] privateKey)
    {
        var result = new ES256KSignatureKey();
        result.Import(publicKey, privateKey);
        return result;
    }

    public static ES256KSignatureKey From(JsonWebKey publicJwk, JsonWebKey privateJwk)
    {
        var result = new ES256KSignatureKey();
        result.Import(publicJwk, privateJwk);
        return result;
    }
}

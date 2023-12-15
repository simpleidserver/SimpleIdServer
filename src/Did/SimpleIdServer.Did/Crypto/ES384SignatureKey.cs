// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Security.Cryptography;

namespace SimpleIdServer.Did.Crypto;

public class ES384SignatureKey : BaseESSignatureKey
{
    private ES384SignatureKey(ECDsa key) : base(key)
    {
    }

    public override string Name => Constants.SupportedSignatureKeyAlgs.ES384;

    protected override string CurveName => "P-384";

    public static ES384SignatureKey New()
    {
        var key = ECDsa.Create(ECCurve.NamedCurves.nistP384);
        return new ES384SignatureKey(key);
    }
}

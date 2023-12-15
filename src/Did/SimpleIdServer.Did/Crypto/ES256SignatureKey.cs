// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Security.Cryptography;

namespace SimpleIdServer.Did.Crypto;

public class ES256SignatureKey : BaseESSignatureKey
{
    private ES256SignatureKey(ECDsa key) : base(key)
    {
    }

    public override string Name => Constants.SupportedSignatureKeyAlgs.ES256;

    protected override string CurveName => "P-256";

    public static ES256SignatureKey New()
    {
        var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        return new ES256SignatureKey(key);
    }
}

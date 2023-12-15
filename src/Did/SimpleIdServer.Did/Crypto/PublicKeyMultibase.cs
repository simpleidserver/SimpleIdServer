// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Did.Crypto;

public class PublicKeyMultibase
{
    public static Dictionary<(string, byte[]), string> AlgToMulticodec = new Dictionary<(string, byte[]), string>
    {
        { ("Q3s", new byte[] { 231, 1 }),  Did.Constants.SupportedSignatureKeyAlgs.ES256K },
        { ("6Mk", new byte[] { 237, 1 }), Did.Constants.SupportedSignatureKeyAlgs.Ed25519 }
    };

    public static string Compute(ISignatureKey signatureKey)
    {
        var kvp = AlgToMulticodec.First(kvp => kvp.Value == signatureKey.Name);
        var publicKeyPayload = signatureKey.GetPublicKey(true);
        var payload = new List<byte>();
        payload.AddRange(kvp.Key.Item2);
        payload.AddRange(publicKeyPayload);
        var publicKeyMultibase = $"z{Encoding.Base58Encoding.Encode(payload.ToArray())}";
        return publicKeyMultibase;
    }
}

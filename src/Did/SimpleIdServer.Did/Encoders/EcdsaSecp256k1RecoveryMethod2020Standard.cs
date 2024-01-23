// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Models;
using System.Collections.Generic;

namespace SimpleIdServer.Did.Encoders;

public class EcdsaSecp256k1RecoveryMethod2020Standard : IVerificationMethodStandard
{
    public const string TYPE = "EcdsaSecp256k1RecoveryMethod2020";
    public const string JSON_LD_CONTEXT = "https://w3id.org/security/suites/secp256k1recovery-2020/v2";

    public string JSONLDContext => JSON_LD_CONTEXT;

    public string Type => TYPE;

    public SignatureKeyEncodingTypes DefaultEncoding => SignatureKeyEncodingTypes.JWK;

    public SignatureKeyEncodingTypes SupportedEncoding => SignatureKeyEncodingTypes.JWK |
        SignatureKeyEncodingTypes.HEX;

    public IEnumerable<string> SupportedCurves => new List<string>
    {
        Constants.StandardCrvOrSize.SECP256k1
    };

    public string BuildId(DidDocumentVerificationMethod verificationMethod, IAsymmetricKey asymmKey)
    {
        return null;
    }
}
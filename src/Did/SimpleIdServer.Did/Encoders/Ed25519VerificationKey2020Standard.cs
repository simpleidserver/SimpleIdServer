// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Models;
using System.Collections.Generic;

namespace SimpleIdServer.Did.Encoders;

/// <summary>
/// https://www.w3.org/community/reports/credentials/CG-FINAL-di-eddsa-2020-20220724/
/// </summary>
public class Ed25519VerificationKey2020Standard : IVerificationMethodStandard
{
    public const string JSON_LD_CONTEXT = "https://w3id.org/security/suites/ed25519-2020/v1";
    public const string TYPE = "Ed25519VerificationKey2020";

    public string JSONLDContext => JSON_LD_CONTEXT;

    public string Type => TYPE;

    public SignatureKeyEncodingTypes DefaultEncoding => SignatureKeyEncodingTypes.MULTIBASE;

    public SignatureKeyEncodingTypes SupportedEncoding => SignatureKeyEncodingTypes.MULTIBASE;

    public IEnumerable<string> SupportedCurves => new List<string>
    {
        Constants.StandardCrvOrSize.Ed25519
    };

    public string BuildId(DidDocumentVerificationMethod verificationMethod, IAsymmetricKey asymmKey)
    {
        return verificationMethod.PublicKeyMultibase;
    }
}
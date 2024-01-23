// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Crypto.Multicodec;
using SimpleIdServer.Did.Models;
using System.Collections.Generic;

namespace SimpleIdServer.Did.Encoders;

public class X25519KeyAgreementKey2019Standard : IVerificationMethodStandard
{
    private readonly IMulticodecSerializer _multicodecSerializer;
    public const string JSON_LD_CONTEXT = "https://w3id.org/security/suites/x25519-2019/v1";
    public const string TYPE = "X25519KeyAgreementKey2019";

    public X25519KeyAgreementKey2019Standard(
        IMulticodecSerializer multicodecSerializer)
    {
        _multicodecSerializer = multicodecSerializer;
    }

    public string JSONLDContext => JSON_LD_CONTEXT;

    public string Type => TYPE;

    public SignatureKeyEncodingTypes DefaultEncoding => SignatureKeyEncodingTypes.BASE58;

    public SignatureKeyEncodingTypes SupportedEncoding => SignatureKeyEncodingTypes.BASE58;

    public IEnumerable<string> SupportedCurves => new List<string>
    {
        Constants.StandardCrvOrSize.X25519
    };

    public string BuildId(DidDocumentVerificationMethod verificationMethod, IAsymmetricKey asymmKey)
    {
        return _multicodecSerializer.SerializePublicKey(asymmKey);
    }
}
// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Extensions;
using SimpleIdServer.Did.Models;

namespace SimpleIdServer.Did.Builders;

/// <summary>
/// Documentation : https://www.w3.org/community/reports/credentials/CG-FINAL-lds-jws2020-20220721/
/// </summary>
public class JWKVerificationMethodFormatter : IVerificationMethodFormatter
{
    public const string JSON_LD_CONTEXT = "https://w3id.org/security/suites/jws-2020/v1";

    public string JSONLDContext => JSON_LD_CONTEXT;

    public string Type => "JsonWebKey2020";

    public DidDocumentVerificationMethod Format(DidDocument idDocument, ISignatureKey signatureKey)
    {
        var publicJWK = signatureKey.GetPublicJwk();
        return new DidDocumentVerificationMethod
        {
            Id = $"{idDocument.Id}#{publicJWK.ComputeJwkThumbprint().ToHex()}",
            PublicKeyJwk = publicJWK
        };
    }

    public ISignatureKey Extract(DidDocumentVerificationMethod didDocumentVerificationMethod)
    {

        throw new System.NotImplementedException();
    }
}

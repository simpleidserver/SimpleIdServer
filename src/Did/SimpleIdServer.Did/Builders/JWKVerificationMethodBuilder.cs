// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Extensions;
using SimpleIdServer.Did.Models;

namespace SimpleIdServer.Did.Builders;

public class JWKVerificationMethodBuilder : IVerificationMethodBuilder
{
    public const string JSON_LD_CONTEXT = "https://w3id.org/security/suites/jws-2020/v1";

    public string JSONLDContext => JSON_LD_CONTEXT;

    public string Type => "JsonWebKey2020";

    public IdentityDocumentVerificationMethod Build(IdentityDocument idDocument, ISignatureKey signatureKey)
    {
        return new IdentityDocumentVerificationMethod
        {
            Id = $"{idDocument.Id}#{signatureKey.GetPublicKey().ToHex()}",
            PublicKeyJwk = signatureKey.GetPublicKeyJwk()
        };
    }
}

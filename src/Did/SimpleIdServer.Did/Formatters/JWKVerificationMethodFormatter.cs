// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Crypto.Multicodec;
using SimpleIdServer.Did.Extensions;
using SimpleIdServer.Did.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace SimpleIdServer.Did.Builders;

/// <summary>
/// Documentation : https://www.w3.org/community/reports/credentials/CG-FINAL-lds-jws2020-20220721/
/// </summary>
public class JWKVerificationMethodFormatter : IVerificationMethodFormatter
{
    private readonly IEnumerable<IVerificationMethod> _verificationMethods;
    public const string JSON_LD_CONTEXT = "https://w3id.org/security/suites/jws-2020/v1";

    public JWKVerificationMethodFormatter(IEnumerable<IVerificationMethod> verificationMethods)
    {
        _verificationMethods = verificationMethods;
    }

    public string JSONLDContext => JSON_LD_CONTEXT;

    public string Type => "JsonWebKey2020";

    public DidDocumentVerificationMethod Format(DidDocument idDocument, IAsymmetricKey signatureKey)
    {
        var publicJWK = signatureKey.GetPublicJwk();
        var id = string.Empty;
        if(signatureKey.Kty == Constants.StandardKty.OKP)
        {
            id = $"{idDocument.Id}#{ComputeOKPPhumbprint(publicJWK).ToHex()}";
        }
        else
        {
            id = $"{idDocument.Id}#{publicJWK.ComputeJwkThumbprint().ToHex()}";
        }

        return new DidDocumentVerificationMethod
        {
            Id = id,
            PublicKeyJwk = publicJWK
        };
    }

    public IAsymmetricKey Extract(DidDocumentVerificationMethod didDocumentVerificationMethod)
    {
        return _verificationMethods.Single(v =>
            v.Kty == didDocumentVerificationMethod.PublicKeyJwk.Kty &&
            v.CrvOrSize == didDocumentVerificationMethod.PublicKeyJwk.Crv)
            .Build(didDocumentVerificationMethod.PublicKeyJwk);

    }

    private byte[] ComputeOKPPhumbprint(JsonWebKey jwk)
    {
        var canonicalJwk = $@"{{""{JsonWebKeyParameterNames.Crv}"":""{jwk.Crv}"",""{JsonWebKeyParameterNames.Kty}"":""{jwk.Kty}"",""{JsonWebKeyParameterNames.X}"":""{jwk.X}""}}";
        return GenerateSha256Hash(canonicalJwk);
    }

    private static byte[] GenerateSha256Hash(string input)
    {
        using (var hash = SHA256.Create())
        {
            return hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        }
    }
}

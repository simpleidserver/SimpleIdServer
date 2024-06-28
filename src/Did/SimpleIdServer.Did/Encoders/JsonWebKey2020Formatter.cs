// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Extensions;
using SimpleIdServer.Did.Models;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SimpleIdServer.Did.Encoders;

/// <summary>
/// Documentation : https://www.w3.org/community/reports/credentials/CG-FINAL-lds-jws2020-20220721/
/// </summary>
public class JsonWebKey2020Standard : IVerificationMethodStandard
{
    public const string TYPE = "JsonWebKey2020";
    public const string JSON_LD_CONTEXT = "https://w3id.org/security/suites/jws-2020/v1";

    public string JSONLDContext => JSON_LD_CONTEXT;

    public string Type => TYPE;

    public SignatureKeyEncodingTypes DefaultEncoding => SignatureKeyEncodingTypes.JWK;

    public SignatureKeyEncodingTypes SupportedEncoding => SignatureKeyEncodingTypes.JWK;


    public IEnumerable<string> SupportedCurves { get; set; } = null;

    public string BuildId(DidDocumentVerificationMethod verificationMethod, IAsymmetricKey asymmKey)
    {
        var publicJWK = asymmKey.GetPublicJwk();
        if (asymmKey.Kty == Constants.StandardKty.OKP)
            return ComputeOKPPhumbprint(publicJWK).ToHex();

        return publicJWK.ComputeJwkThumbprint().ToHex();
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

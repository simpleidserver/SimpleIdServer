// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Did.Builders;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Vc.Canonize;
using SimpleIdServer.Vc.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;

namespace SimpleIdServer.Vc.Proofs;

/// <summary>
/// https://www.w3.org/community/reports/credentials/CG-FINAL-lds-jws2020-20220721/
/// </summary>
public class JsonWebSignature2020Proof : ISignatureProof
{
    public string Type => "JsonWebSignature2020";

    public string JsonLdContext = JsonWebKey2020Formatter.JSON_LD_CONTEXT;

    public string VerificationMethod => JsonWebKey2020Formatter.TYPE;

    public string TransformationMethod => RdfCanonize.NAME;

    public HashAlgorithmName HashingMethod => HashAlgorithmName.SHA256;

    public void ComputeProof(DataIntegrityProof proof, byte[] payload, IAsymmetricKey asymmetricKey, HashAlgorithmName alg)
    {
        var signature = asymmetricKey.SignHash(payload, alg);
        var jwtHeader = BuildJwtHeader(asymmetricKey);
        var jwtSignature = BuildJwtSignature(signature);
        proof.Jws = $"{jwtHeader}..{jwtSignature}";
    }

    public byte[] GetSignature(DataIntegrityProof proof)
    {
        var result = proof.Jws.Split('.');
        var jwtSignature = result.Last();
        return Base64UrlEncoder.DecodeBytes(jwtSignature);
    }

    private string BuildJwtHeader(IAsymmetricKey asymmetricKey)
    {
        var result = new JsonObject();
        var arr = new JsonArray();
        arr.Add("b64");
        result.Add("alg", "EdDSA");
        result.Add("b64", false);
        result.Add("crit", arr);
        var json = result.ToJsonString();
        return Base64UrlEncoder.Encode(Encoding.UTF8.GetBytes(json));
    }

    private string BuildJwtSignature(byte[] signature)
    {
        return Base64UrlEncoder.Encode(signature);
    }
}

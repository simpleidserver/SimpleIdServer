// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Crypto.Multicodec;
using SimpleIdServer.Did.Encoders;
using SimpleIdServer.Did.Models;
using SimpleIdServer.Vc.Canonize;
using SimpleIdServer.Vc.CredentialFormats.Serializers;
using SimpleIdServer.Vc.Hashing;
using SimpleIdServer.Vc.Models;
using SimpleIdServer.Vc.Proofs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Vc;

public class SecuredVerifiableCredential
{
    private readonly IEnumerable<ISignatureProof> _proofs;
    private readonly IEnumerable<ICanonize> _canonizeMethods;
    private readonly IEnumerable<IHashing> _hashingMethods;
    private readonly IVerificationMethodEncoding _verificationMethodEncoding;

    private SecuredVerifiableCredential(
        IEnumerable<ISignatureProof> proofs,
        IEnumerable<ICanonize> canonizeMethods,
        IEnumerable<IHashing> hashingMethods,
        IVerificationMethodEncoding verificationMethodEncoding)
    {
        _proofs = proofs;
        _canonizeMethods = canonizeMethods;
        _hashingMethods = hashingMethods;
        _verificationMethodEncoding = verificationMethodEncoding;
    }

    public static SecuredVerifiableCredential New()
    {
        return new SecuredVerifiableCredential(new ISignatureProof[]
        {
            new Ed25519Signature2020Proof(),
            new JsonWebSignature2020Proof()
        }, new ICanonize[]
        {
            new RdfCanonize()
        }, new IHashing[]
        {
            new SHA256Hash()
        }, new VerificationMethodEncoding(VerificationMethodStandardFactory.GetAll(), MulticodecSerializerFactory.Build(), MulticodecSerializerFactory.AllVerificationMethods));
    }

    public string Secure(
        string json, 
        DidDocument didDocument, 
        string verificationMethodId, 
        ProofPurposeTypes purpose = ProofPurposeTypes.assertionMethod, 
        IAsymmetricKey asymKey = null,
        DateTime? creationDateTime = null)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentNullException(nameof(json));
        if (didDocument == null) throw new ArgumentNullException(nameof(didDocument));
        var jObj = JsonObject.Parse(json).AsObject();
        if (jObj == null) throw new ArgumentException("not a valid JSON");
        if (string.IsNullOrWhiteSpace(verificationMethodId)) throw new ArgumentNullException(verificationMethodId);
        var verificationMethod = didDocument.VerificationMethod.SingleOrDefault(m => m.Id == verificationMethodId);
        if (verificationMethod == null) throw new ArgumentException($"The verification method {verificationMethodId} doesn't exist");
        var proof = _proofs.SingleOrDefault(p => p.VerificationMethod == verificationMethod.Type);
        if (proof == null) throw new InvalidOperationException($"Impossible to produce a proof for the verification method {verificationMethod.Type}");
        var created = creationDateTime ?? DateTime.UtcNow;
        var dataIntegrityProof = new DataIntegrityProof
        {
            Type = proof.Type,
            Created = created,
            ProofPurpose = purpose,
            VerificationMethod = verificationMethod.Id
        };
        var hashPayload = HashDocument(jObj, proof);
        var hashProof = HashProof(dataIntegrityProof, proof, jObj);
        var result = new List<byte>();
        result.AddRange(hashProof);
        result.AddRange(hashPayload);
        // 3. Signature
        if(asymKey == null)
            asymKey = _verificationMethodEncoding.Decode(verificationMethod);
        proof.ComputeProof(dataIntegrityProof, result.ToArray(), asymKey, proof.HashingMethod);
        jObj.Add("proof", JsonObject.Parse(JsonSerializer.Serialize(dataIntegrityProof, typeof(DataIntegrityProof), GetJsonOptions())));
        return jObj.ToString();
    }

    public bool Check(string json, DidDocument didDocument)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentNullException(nameof(json));
        if (didDocument == null) throw new ArgumentNullException(nameof(didDocument));
        var jObj = JsonObject.Parse(json).AsObject();
        if (jObj == null) throw new ArgumentException("not a valid JSON");
        if (!jObj.ContainsKey("proof")) throw new InvalidOperationException("The JSON doesn't contain a proof");
        var proofJson = jObj["proof"].AsObject().ToString();
        var dataIntegrityProof = JsonSerializer.Deserialize<DataIntegrityProof>(proofJson);
        var emptyDataIntegrityProof = JsonSerializer.Deserialize<DataIntegrityProof>(proofJson);
        emptyDataIntegrityProof.ProofValue = null;
        emptyDataIntegrityProof.Jws = null;
        var verificationMethod = didDocument.VerificationMethod.SingleOrDefault(m => m.Id == dataIntegrityProof.VerificationMethod);
        if (verificationMethod == null) throw new InvalidOperationException($"The verification method {dataIntegrityProof.VerificationMethod} doesn't exist in the DID Document");
        var proof = _proofs.SingleOrDefault(p => p.VerificationMethod == verificationMethod.Type);
        var hashPayload = HashDocument(jObj, proof);
        var hashProof = HashProof(emptyDataIntegrityProof, proof, jObj);
        var result = new List<byte>();
        result.AddRange(hashProof);
        result.AddRange(hashPayload);
        // 3. CheckHash signature.
        var signature = proof.GetSignature(dataIntegrityProof);
        var asymKey = _verificationMethodEncoding.Decode(verificationMethod);
        return asymKey.CheckHash(result.ToArray(), signature, proof.HashingMethod);
    }

    public string SecureJwt(
        string issuer,
        DidDocument didDocument,
        string verificationMethodId,
        W3CVerifiableCredential vcCredential,
        IAsymmetricKey asymKey = null)
    {
        if (didDocument == null) throw new ArgumentNullException(nameof(didDocument));
        if (string.IsNullOrWhiteSpace(verificationMethodId)) throw new ArgumentNullException(nameof(verificationMethodId));
        if (vcCredential == null) throw new ArgumentNullException(nameof(vcCredential));
        var verificationMethod = didDocument.VerificationMethod.SingleOrDefault(m => m.Id == verificationMethodId);
        if (verificationMethod == null) throw new ArgumentException($"The verification method {verificationMethodId} doesn't exist");
        if(asymKey == null)
            asymKey = _verificationMethodEncoding.Decode(verificationMethod);
        var signingCredentials = asymKey.BuildSigningCredentials();
        var claims = new Dictionary<string, object>
        {
            { "sub", didDocument.Id },
            { "vc", new W3CVerifiableCredentialJsonSerializer().SerializeToDic(vcCredential) }
        };
        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            IssuedAt = DateTime.UtcNow,
            SigningCredentials = signingCredentials,
            Claims = claims
        };
        var handler = new JsonWebTokenHandler();
        var result = handler.CreateToken(securityTokenDescriptor);
        return result;
    }

    private byte[] HashDocument(JsonObject jObj, ISignatureProof proof)
    {
        if (jObj.ContainsKey("proof")) jObj.Remove("proof");
        var json = jObj.ToString();
        return Hash(json, proof);
    }

    private byte[] HashProof(DataIntegrityProof dataIntegrityProof, ISignatureProof proof, JsonObject jObj)
    {
        var json = JsonSerializer.Serialize(dataIntegrityProof, typeof(DataIntegrityProof), GetJsonOptions());
        if (jObj.ContainsKey("@context"))
        {
            var j = JsonObject.Parse(json).AsObject();
            var context = jObj["@context"].ToJsonString();
            j.Add("@context", JsonNode.Parse(context));
            json = j.ToJsonString();
        }

        return Hash(json, proof);
    }

    private byte[] Hash(string json, ISignatureProof proof)
    {
        // 1. Transform.
        var canonizeMethod = _canonizeMethods.Single(m => m.Name == proof.TransformationMethod);
        var canonized = canonizeMethod.Transform(json);
        var payload = Encoding.UTF8.GetBytes(canonized);
        // 2. Hash.
        var hashingMethod = _hashingMethods.Single(m => m.Name == proof.HashingMethod);
        var hashPayload = hashingMethod.Hash(payload);
        return hashPayload;
    }
    
    private JsonSerializerOptions GetJsonOptions()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new IsoDateTimeJsonConverter());
        return options;
    }
}

public class IsoDateTimeJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Debug.Assert(typeToConvert == typeof(DateTime));
        return DateTime.Parse(reader.GetString() ?? string.Empty);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"));
    }
}
// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Encoding;
using SimpleIdServer.Did.Formatters;
using SimpleIdServer.Did.Models;
using SimpleIdServer.Vc.Canonize;
using SimpleIdServer.Vc.Hashing;
using SimpleIdServer.Vc.Models;
using SimpleIdServer.Vc.Proofs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.Vc;

public class SecuredVerifiableCredential
{
    private readonly IEnumerable<ISignatureProof> _proofs;
    private readonly IEnumerable<ICanonize> _canonizeMethods;
    private readonly IEnumerable<IHashing> _hashingMethods;
    private readonly IFormatterFactory _formatterFactory;

    private SecuredVerifiableCredential(
        IEnumerable<ISignatureProof> proofs,
        IEnumerable<ICanonize> canonizeMethods,
        IEnumerable<IHashing> hashingMethods,
        IFormatterFactory formatterFactory)
    {
        _proofs = proofs;
        _canonizeMethods = canonizeMethods;
        _hashingMethods = hashingMethods;
        _formatterFactory = formatterFactory;
    }

    public static SecuredVerifiableCredential New()
    {
        return new SecuredVerifiableCredential(new ISignatureProof[]
        {
            new Ed25519Signature2020Proof()
        }, new ICanonize[]
        {
            new RdfCanonize()
        }, new IHashing[]
        {
            new SHA256Hash()
        }, new FormatterFactory());
    }

    public string Secure(string json, DidDocument didDocument, string verificationMethodId, string purpose = "assertionMethod")
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentNullException(nameof(json));
        if (didDocument == null) throw new ArgumentNullException(nameof(didDocument));
        if (string.IsNullOrWhiteSpace(verificationMethodId)) throw new ArgumentNullException(verificationMethodId);
        var verificationMethod = didDocument.VerificationMethod.SingleOrDefault(m => m.Id == verificationMethodId);
        if (verificationMethod == null) throw new ArgumentException($"The verification method {verificationMethodId} doesn't exist");
        var proof = _proofs.SingleOrDefault(p => p.VerificationMethod == verificationMethod.Type);
        if (proof == null) throw new InvalidOperationException($"Impossible to produce a proof for the verification method {verificationMethod.Type}");
        var jObj = JsonObject.Parse(json).AsObject();
        if (jObj.ContainsKey("proof")) jObj.Remove("proof");
        // 1. Transform.
        var canonizeMethod = _canonizeMethods.Single(m => m.Name == proof.TransformationMethod);
        var canonized = canonizeMethod.Transform(json);
        var payload = Encoding.UTF8.GetBytes(canonized);
        // 2. Hash.
        var hashingMethod = _hashingMethods.Single(m => m.Name == proof.HashingMethod);
        var hashPayload = hashingMethod.Hash(payload);
        // 3. Signature
        var formatter = _formatterFactory.ResolveFormatter(verificationMethod);
        var asymKey = formatter.Extract(verificationMethod);
        var signature = asymKey.Sign(hashPayload);
        var proofValue = MultibaseEncoding.Encode(signature);
        var dataIntegrityProof = new DataIntegrityProof
        {
            Type = proof.Type,
            Created = DateTime.UtcNow,
            ProofPurpose = purpose,
            VerificationMethod = verificationMethod.Id,
            ProofValue = proofValue
        };
        jObj.Add("proof", JsonObject.Parse(JsonSerializer.Serialize(dataIntegrityProof)));
        return jObj.ToString();
    }

    public void Check(string json, DidDocument didDocument)
    {
        // Check proof exists.
        // Check verification method exists.
        // Get payload (transformation + hash).
        // Check signature.
        
    }
}

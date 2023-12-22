// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Encoding;
using SimpleIdServer.Did.Models;
using SimpleIdServer.Vc.Canonize;
using System;
using System.Security.Cryptography;
using System.Text;

namespace SimpleIdServer.Vc;

public class SecuredVerifiableCredential
{
    private readonly ICanonize _canonize;
    

    public SecuredVerifiableCredential()
    {
        _canonize = new RdfCanonize();
    }

    public void Secure(string json, DidDocument didDocument, string verificationMethodId)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentNullException(nameof(json));
        if (didDocument == null) throw new ArgumentNullException(nameof(didDocument));
        if (string.IsNullOrWhiteSpace(verificationMethodId)) throw new ArgumentNullException(verificationMethodId);
        // TODO : get signature proof.
        var canonized = _canonize.Transform(json);
        var payload = Encoding.UTF8.GetBytes(canonized);
        var hashPayload = Hash(payload);
        var proofValue = MultibaseEncoding.Encode(hashPayload);
        // Add a proofValue.
    }

    private byte[] Hash(byte[] input)
    {
        using (var sha256 = SHA256.Create())
        {
            return sha256.ComputeHash(input);
        }
    }
}

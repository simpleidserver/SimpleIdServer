// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Models;
using SimpleIdServer.Vc;
using System;
using System.Text.Json.Nodes;

namespace SimpleIdServer.CredentialIssuer.CredentialFormats;

/// <summary>
/// VC signed as a JWT, not using JSON-LD.
/// </summary>
public class JwtVcJsonFormatter : BaseW3CVerifiableCredentialFormatter
{
    public override string Format => "jwt_vc_json";

    public override JsonNode Build(BuildCredentialRequest request, DidDocument didDocument, string verificationMethodId)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (didDocument == null) throw new ArgumentNullException(nameof(didDocument));
        var credential = BuildCredential(request, false);
        return SecuredVerifiableCredential.New()
            .SecureJwt(request.Issuer, didDocument, verificationMethodId, credential);
    }
}
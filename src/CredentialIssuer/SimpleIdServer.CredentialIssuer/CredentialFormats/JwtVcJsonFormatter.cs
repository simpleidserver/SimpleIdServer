// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Crypto;
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
    public override string Format => FORMAT;

    public const string FORMAT = "jwt_vc_json";

    public override JsonNode Build(BuildCredentialRequest request, DidDocument didDocument, string verificationMethodId, IAsymmetricKey asymmetricKey)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (didDocument == null) throw new ArgumentNullException(nameof(didDocument));
        if (verificationMethodId == null) throw new ArgumentNullException(nameof(verificationMethodId));
        if (asymmetricKey == null) throw new ArgumentNullException(nameof(asymmetricKey));
        var credential = BuildCredential(request);
        return SecuredDocument.New()
            .SecureJwt(request.RequestSubject, didDocument, verificationMethodId, credential, asymKey: asymmetricKey);
    }
}
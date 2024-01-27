// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Models;
using SimpleIdServer.Vc;
using System;

namespace SimpleIdServer.CredentialIssuer.CredentialFormats;

/// <summary>
/// VC signed as a JWT, using JSON-LD.
/// </summary>
public class JwtVcJsonLdFormatter : BaseW3CVerifiableCredentialFormatter
{
    public override string Format => "jwt_vc_json-ld";

    public string Build(BuildCredentialRequest request, DidDocument didDocument, string verificationMethodId)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (didDocument == null) throw new ArgumentNullException(nameof(didDocument));
        var credential = BuildCredential(request, true);
        return SecuredVerifiableCredential.New()
            .SecureJwt(request.Issuer, didDocument, verificationMethodId, credential);
    }
}
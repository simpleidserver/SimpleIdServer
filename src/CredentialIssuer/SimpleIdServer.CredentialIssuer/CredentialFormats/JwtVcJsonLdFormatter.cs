// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Nodes;

namespace SimpleIdServer.CredentialIssuer.CredentialFormats;

/// <summary>
/// VC signed as a JWT, using JSON-LD.
/// </summary>
public class JwtVcJsonLdFormatter : BaseW3CVerifiableCredentialFormatter
{
    public override string Format => "jwt_vc_json-ld";
}
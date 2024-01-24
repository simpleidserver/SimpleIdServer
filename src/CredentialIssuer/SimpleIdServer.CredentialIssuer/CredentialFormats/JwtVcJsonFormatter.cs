// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Nodes;

namespace SimpleIdServer.CredentialIssuer.CredentialFormats;

public class JwtVcJsonFormatter : ICredentialFormatter
{
    public string Format => "jwt_vc_json";

    public CredentialHeader ExtractHeader(JsonObject jsonObj)
    {
        return null;
    }

    public void BuildCredential()
    {

    }
}
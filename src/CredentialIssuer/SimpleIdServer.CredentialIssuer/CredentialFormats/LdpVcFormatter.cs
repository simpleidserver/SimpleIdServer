// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Nodes;

namespace SimpleIdServer.CredentialIssuer.CredentialFormats;

public class LdpVcFormatter : ICredentialFormatter
{
    public string Format => "ldp_vc";

    public CredentialHeader ExtractHeader(JsonObject jsonObj)
    {
        throw new System.NotImplementedException();
    }
}

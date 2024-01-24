// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.CredentialIssuer.Domains;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace SimpleIdServer.CredentialIssuer.CredentialFormats;

public interface ICredentialDefinitionSerializer
{
    JsonObject Serialize(CredentialTemplate template);
}

public class CredentialDefinitionSerializer
{
    private readonly IEnumerable<ICredentialFormatter> _credentialSerializers;

    public CredentialDefinitionSerializer(IEnumerable<ICredentialFormatter> credentialSerializers)
    {
        _credentialSerializers = credentialSerializers;
    }

    public JsonObject Serialize(CredentialTemplate template)
    {
        if (template == null) throw new ArgumentNullException(nameof(template));
        return null;
    }
}
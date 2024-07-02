// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.OpenidFederation.Apis.OpenidFederation;

namespace SimpleIdServer.OpenidFederation;

public class OpenidTrustChain
{
    public OpenidTrustChain(List<OpenidFederationResult> entityStatements)
    {
        EntityStatements = entityStatements;
    }

    public List<OpenidFederationResult> EntityStatements { get; private set; }

    public OpenidTrustChainValidationResult Validate()
    {
        // EntityStatements.Any(c => c.Iat );
        return null;
    }
}
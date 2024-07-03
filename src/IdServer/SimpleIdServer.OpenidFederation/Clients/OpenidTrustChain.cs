// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.OpenidFederation.Apis.OpenidFederation;
using SimpleIdServer.OpenidFederation.Resources;

namespace SimpleIdServer.OpenidFederation.Clients;

public class OpenidTrustChain
{
    public OpenidTrustChain(List<OpenidFederationResult> entityStatements)
    {
        EntityStatements = entityStatements;
    }

    public List<OpenidFederationResult> EntityStatements { get; private set; }

    public OpenidTrustChainValidationResult Validate()
    {
        var validationResult = new OpenidTrustChainValidationResult();
        var currentDateTime = DateTime.UtcNow;
        foreach(var entityStatement in EntityStatements)
        {
            if (entityStatement.ValidTo != null && entityStatement.ValidTo < currentDateTime)
                validationResult.AddError(string.Format(Global.EntityStatementIsExpired, entityStatement.Sub));
        }

        return validationResult;
    }
}
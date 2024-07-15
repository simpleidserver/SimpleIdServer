// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.OpenidFederation.Apis.OpenidFederation;
using SimpleIdServer.OpenidFederation.Resources;

namespace SimpleIdServer.OpenidFederation.Clients;

public class OpenidTrustChain
{
    public OpenidTrustChain(List<EntityStatement> entityStatements, string path)
    {
        EntityStatements = entityStatements;
        Path = path;
    }

    public List<EntityStatement> EntityStatements { get; private set; }
    public string Path { get; set; }
    public EntityStatement? TrustAnchor
    {
        get
        {
            return EntityStatements.LastOrDefault();
        }
    }

    public OpenidTrustChainValidationResult Validate()
    {
        var validationResult = new OpenidTrustChainValidationResult();
        var currentDateTime = DateTime.UtcNow;
        for (var i = 0; i < EntityStatements.Count; i++)
        {
            var entityStatement = EntityStatements.ElementAt(i);
            if (entityStatement.FederationResult.ValidTo != null && entityStatement.FederationResult.ValidTo < currentDateTime)
                validationResult.AddError(string.Format(Global.EntityStatementIsExpired, entityStatement.FederationResult.Sub));

            if(i == 0 || i == EntityStatements.Count() - 1)
            {
                if (entityStatement.FederationResult.Iss != entityStatement.FederationResult.Sub)
                    validationResult.AddError(Global.IssuerMustBeEqualsToSubject);

                if (!VerifySignature(entityStatement.Jws, entityStatement.FederationResult))
                    validationResult.AddError(string.Format(Global.EntityStatementSignatureIsInvalid, entityStatement.FederationResult.Sub));
            }
            else
            {
                var nextEntityStatement = EntityStatements.ElementAt(i + 1);
                if (entityStatement.FederationResult.Iss != nextEntityStatement.FederationResult.Sub)
                    validationResult.AddError(Global.SubDifferentToPreviousIssuer);
                if (!VerifySignature(entityStatement.Jws, nextEntityStatement.FederationResult))
                    validationResult.AddError(string.Format(Global.EntityStatementSignatureIsInvalid, nextEntityStatement.FederationResult.Sub));
            }
        }

        return validationResult;
    }

    private bool VerifySignature(string jws, OpenidFederationResult federationResult)
    {
        var handler = new JsonWebTokenHandler();
        var jsonWebKey = new JsonWebKey(federationResult.Jwks.JsonWebKeys.Single().ToString());
        var validationResult = handler.ValidateToken(jws, new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateLifetime = false,
            ValidateAudience = false,
            IssuerSigningKey = jsonWebKey
        });
        return validationResult.IsValid;
    }
}

public class EntityStatement
{
    public EntityStatement(string jws, OpenidFederationResult federationResult)
    {
        Jws = jws;
        FederationResult = federationResult;
    }

    public string Jws { get; private set; } = null!;
    public OpenidFederationResult FederationResult { get; private set; } = null!;
}
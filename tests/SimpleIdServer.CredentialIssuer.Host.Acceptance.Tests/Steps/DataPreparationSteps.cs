// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Jwt;
using SimpleIdServer.Did.Key;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SimpleIdServer.CredentialIssuer.Host.Acceptance.Tests.Steps;

[Binding]
public class DataPreparationSteps
{
    private readonly ScenarioContext _scenarioContext;

    public DataPreparationSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given("build jwt proof")]
    public async Task GivenBuildJwtProof(Table table)
    {
        var ed25519 = Ed25519SignatureKey.Generate();
        var generator = DidKeyGenerator.New();
        var resolver = DidKeyResolver.New();
        var securedVc = DidJsonWebTokenHandler.New();
        var did = generator.Generate(ed25519);
        var didDocument = await resolver.Resolve(did, CancellationToken.None);
        var claims = new Dictionary<string, object>();
        string tokenType = null, kid = didDocument.VerificationMethod.First().Id;
        foreach(var row in table.Rows)
        {
            var key = row["Key"];
            var value = row["Value"];
            if (key == "typ") tokenType = value;
            else if (key == "kid") kid = value;
            else claims.Add(row["Key"], row["Value"]);
        }

        var jwtProof = securedVc.Secure(claims, ed25519, tokenType, kid);
        _scenarioContext.Set(jwtProof, "jwtProof");
    }

    [Given("access token contains one credential identifier '(.*)'")]
    public void GivenAccessTokenContainsCredentialIdentifiers(string credentialIdentifier)
    {
        _scenarioContext.Set(credentialIdentifier, "credentialIdentifier");
    }
}

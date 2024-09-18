// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using TechTalk.SpecFlow;

namespace SimpleIdServer.FastFed.Host.Acceptance.Tests.Steps;

[Binding]
public class DataPreparationSteps
{
    private readonly ScenarioContext _scenarioContext;

    public DataPreparationSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given("build jwt and store the result into '(.*)'")]
    public void GivenBuildJwsByUsingRandomRS256SignatureKey(string key, Table table)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Claims = new Dictionary<string, object>(),
            SigningCredentials = new SigningCredentials(new RsaSecurityKey(RSA.Create())
            {
                KeyId = "kid"
            }, SecurityAlgorithms.RsaSha256)
        };
        foreach (var row in table.Rows)
            tokenDescriptor.Claims.Add(row["Key"].ToString(), row["Value"].ToString());
        var handler = new JsonWebTokenHandler();
        var jws = handler.CreateToken(tokenDescriptor);
        _scenarioContext.Set(jws, key);
    }

    [Given("build jwt signed with certificate and store the result into '(.*)'")]
    public void BuildJwtAndSignWithCertificate(string key, Table table)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Claims = new Dictionary<string, object>(),
            SigningCredentials = Constants.SigningCredentials
        };
        foreach (var row in table.Rows)
            tokenDescriptor.Claims.Add(row["Key"].ToString(), row["Value"].ToString());
        var handler = new JsonWebTokenHandler();
        var jws = handler.CreateToken(tokenDescriptor);
        _scenarioContext.Set(jws, key);
    }
}

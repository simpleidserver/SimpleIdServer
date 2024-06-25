// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Helpers;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SimpleIdServer.SelfIdServer.Host.Acceptance.Tests.Steps;

[Binding]
public class DataPreparationSteps
{
    private readonly ScenarioContext _scenarioContext;

    public DataPreparationSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given("build JWS by signing with a random RS256 algorithm and store the result into '(.*)'")]
    public void GivenBuildJwsByUsingRandomRS256SignatureKey(string key, Table table)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Claims = new Dictionary<string, object>(),
            SigningCredentials = new SigningCredentials(new RsaSecurityKey(RSA.Create())
            {
                KeyId = Guid.NewGuid().ToString()
            }, SecurityAlgorithms.RsaSha256)
        };
        foreach (var row in table.Rows)
            tokenDescriptor.Claims.Add(row["Key"].ToString(), row["Value"].ToString());
        var handler = new JsonWebTokenHandler();
        var jws = handler.CreateToken(tokenDescriptor);
        _scenarioContext.Set(jws, key);
    }

    [Given("build expiration time and add '(.*)' seconds")]
    public void GivenBuildExpirationTime(int seconds) => _scenarioContext.Set(DateTime.UtcNow.AddSeconds(seconds).ConvertToUnixTimestamp(), "exp");

    [Given("build random X509Certificate2 and store into '(.*)'")]
    public void GivenBuildRandomCertificate(string key)
    {
        var ecdsa = ECDsa.Create();
        var req = new CertificateRequest($"cn={Guid.NewGuid()}", ecdsa, HashAlgorithmName.SHA256);
        var cert = req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(2));
        _scenarioContext.Set(cert, key);
    }
}
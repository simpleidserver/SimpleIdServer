// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using TechTalk.SpecFlow;

namespace SimpleIdServer.OAuth.Host.Acceptance.Tests.Steps
{
    [Binding]
    public class DataPreparationSteps
    {
        private readonly ScenarioContext _scenarioContext;

        public DataPreparationSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given("authenticate a user")]
        public void GivenUserIsAuthenticated()
        {
            _scenarioContext.EnableUserAuthentication();
        }

        [Given("build JWS by signing with a random RS256 algorithm and store the result into '(.*)'")]
        public void GivenBuildJwsByUsingRandomRS256SignatureKey(string key, Table table)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Claims = new Dictionary<string, object>(),
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(new RSACryptoServiceProvider(2048))
                {
                    KeyId = Guid.NewGuid().ToString()
                }, SecurityAlgorithms.RsaSha256)
            };
            foreach(var row in table.Rows) 
                tokenDescriptor.Claims.Add(row["Key"].ToString(), row["Value"].ToString());
            var handler = new JsonWebTokenHandler();
            var jws = handler.CreateToken(tokenDescriptor);
            _scenarioContext.Set(jws, key);
        }
    }
}

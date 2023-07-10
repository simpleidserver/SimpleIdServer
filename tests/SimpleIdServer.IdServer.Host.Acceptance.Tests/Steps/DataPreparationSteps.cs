// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.DPoP;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using TechTalk.SpecFlow;

namespace SimpleIdServer.IdServer.Host.Acceptance.Tests.Steps
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
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(RSA.Create())
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

        [Given("build DPoP proof")]
        public void GivenBuildDPoPProof(Table table)
        {
            var dpopHandler = new DPoPHandler();
            var claims = new List<Claim>();
            foreach (var row in table.Rows) claims.Add(new Claim(row["Key"], row["Value"]));
            var dpopProof = dpopHandler.CreateRSA(claims);
            _scenarioContext.Set(dpopProof.Token, "DPOP");
        }

        [Given("build DPoP proof with big lifetime")]
        public void GivenBuildDPoPProofWithBigLifetime(Table table)
        {
            var dpopHandler = new DPoPHandler();
            var claims = new List<Claim>();
            foreach (var row in table.Rows) claims.Add(new Claim(row["Key"], row["Value"]));
            var dpopProof = dpopHandler.CreateRSA(claims, expiresInSeconds: 500);
            _scenarioContext.Set(dpopProof.Token, "DPOP");
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
}

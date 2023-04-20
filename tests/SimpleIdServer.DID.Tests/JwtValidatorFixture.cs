// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.Did.Jwt;
using SimpleIdServer.Did.Models;
using System.Text.Json;

namespace SimpleIdServer.DID.Tests
{
    public class JwtValidatorFixture
    {
        [Test]
        public void When_Check_JwtToken_Then_SignatureIsCorrect()
        {
            const string jwt = "eyJ0eXAiOiJKV1QiLCJhbGciOiJFUzI1NksifQ.eyJpYXQiOjE0ODUzMjExMzMsImlzcyI6ImRpZDpldGhyOjB4OTBlNDVkNzViZDEyNDZlMDkyNDg3MjAxODY0N2RiYTk5NmE4ZTdiOSIsInJlcXVlc3RlZCI6WyJuYW1lIiwicGhvbmUiXX0.KIG2zUO8Quf3ucb9jIncZ1CmH0v-fAZlsKvesfsd9x4RzU0qrvinVd9d30DOeZOwdwEdXkET_wuPoOECwU0IKA";
            var didDocument = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "didDoc.json"));
            var didDoc = JsonSerializer.Deserialize<IdentityDocument>(didDocument);
            var validator = new JwtValidator();
            Assert.True(validator.Validate(jwt, didDoc));
        }
    }
}

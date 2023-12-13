// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.Did.Models;
using System.Text.Json;

namespace SimpleIdServer.Did.Jwt.Tests
{
    public class JwtValidatorFixture
    {
        [Test]
        public void When_Check_JwtTokenES256K_Then_SignatureIsCorrect()
        {
            const string jwt = "eyJ0eXAiOiJKV1QiLCJhbGciOiJFUzI1NksifQ.eyJpYXQiOjE0ODUzMjExMzMsImlzcyI6ImRpZDpldGhyOjB4OTBlNDVkNzViZDEyNDZlMDkyNDg3MjAxODY0N2RiYTk5NmE4ZTdiOSIsInJlcXVlc3RlZCI6WyJuYW1lIiwicGhvbmUiXX0.KIG2zUO8Quf3ucb9jIncZ1CmH0v-fAZlsKvesfsd9x4RzU0qrvinVd9d30DOeZOwdwEdXkET_wuPoOECwU0IKA";
            var didDocument = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "files", "es256k-publicKeyJwk.json"));
            var didDoc = JsonSerializer.Deserialize<IdentityDocument>(didDocument);
            Assert.True(DidJwtValidator.Validate(jwt, didDoc));
        }

        [Test]
        public void When_Check_JwtTokenED25519_Then_SignatureIsCorrect()
        {
            const string jwt = "eyJ0eXAiOiJKV1QiLCJhbGciOiJFZDI1NTE5In0.eyJpYXQiOjE0ODUzMjExMzMsInJlcXVlc3RlZCI6WyJuYW1lIiwicGhvbmUiXSwiaXNzIjoiZGlkOm5hY2w6QnZyQjhpSkF6XzFqZnExbVJ4aUVLZnI5cWNuTGZxNURPR3JCZjJFUlVIVSJ9.ZoPf01SxW2n5zngunI942FpviEMP6jBZZb9NJ27M_K7AcmjPeeLH8bm2lv0INmJ2u98JVSzELF8YLWQvPYB1Bw";
            var didDocument = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "files", "ed25519-publicKeyBase64.json"));
            var didDoc = JsonSerializer.Deserialize<IdentityDocument>(didDocument);
            Assert.True(DidJwtValidator.Validate(jwt, didDoc));
        }

        [Test]
        public void When_Check_JwtTokenES256_Then_SignatureIsCorrect()
        {
            // https://github.com/decentralized-identity/did-jwt/blob/master/src/__tests__/JWT.test.ts
            const string jwt = "eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJpYXQiOjE0ODUzMjExMzMsInJlcXVlc3RlZCI6WyJuYW1lIiwicGhvbmUiXSwiaXNzIjoiZGlkOmtleTp6RG5hZWo0TkhudGRhNHJOVzRGQlVKZ0Z6ZGNnRUFYS0dSVkdFOEx1VmZSYnVNdWMxIn0.aMYFY0jitx2Bq9_wGBhEeIyVvzr2XkouCyEP662P8TbAPTpXOC3UrGQONaPD7wleLrMhGdvfod7idSxKXLl64Q";
            var didDocument = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "files", "ed256-publicKeyHex.json"));
            var didDoc = JsonSerializer.Deserialize<IdentityDocument>(didDocument);
            Assert.True(DidJwtValidator.Validate(jwt, didDoc));
        }
    }
}

// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.Jwt.Jws.Handlers;
using System.Security.Cryptography;
using Xunit;

namespace SimpleIdServer.Jwt.Tests.Jws
{
    public class JwsGeneratorFixture
    {
        private static IJwsGenerator _jwsGenerator;

        [Fact]
        public void When_Generate_Jws_And_Check_Signature_Then_True_Is_Returned()
        {
            // ARRANGE
            InitializeFakeObjects();
            JsonWebKey rsaJsonWebKey, ecJsonWebKey, hmac256JsonWebKey;
            using (var rsa = RSA.Create())
            {
                rsaJsonWebKey = new JsonWebKeyBuilder().NewSign("keyId", new[]
                {
                    KeyOperations.Sign
                }).SetAlg(rsa, "RS256").Build();
            }

            using (var ec = new ECDsaCng())
            {
                ecJsonWebKey = new JsonWebKeyBuilder().NewSign("keyId", new[]
                {
                    KeyOperations.Sign
                }).SetAlg(ec, "ES256").Build();
            }

            using (var hmac = new HMACSHA256())
            {
                hmac256JsonWebKey = new JsonWebKeyBuilder().NewSign("keyId", new[]
                {
                    KeyOperations.Sign
                }).SetAlg(hmac, "HS256").Build();
            }

            var payload = new JObject();
            payload.Add("sub", "thabart");

            // ACT 
            var rsa256JWS = _jwsGenerator.Build(payload.ToString(), "RS256", rsaJsonWebKey);
            var rsa384JWS = _jwsGenerator.Build(payload.ToString(), "RS384", rsaJsonWebKey);
            var rsa512JWS = _jwsGenerator.Build(payload.ToString(), "RS512", rsaJsonWebKey);
            var ec256JWS = _jwsGenerator.Build(payload.ToString(), "ES256", ecJsonWebKey);
            var ec384JWS = _jwsGenerator.Build(payload.ToString(), "ES384", ecJsonWebKey);
            var ec512JWS = _jwsGenerator.Build(payload.ToString(), "ES512", ecJsonWebKey);
            var hs256JWS = _jwsGenerator.Build(payload.ToString(), "HS256", hmac256JsonWebKey);
            var hs384JWS = _jwsGenerator.Build(payload.ToString(), "HS384", hmac256JsonWebKey);
            var hs512JWS = _jwsGenerator.Build(payload.ToString(), "HS512", hmac256JsonWebKey);
            var ps256JWS = _jwsGenerator.Build(payload.ToString(), "PS256", rsaJsonWebKey);
            var ps384JWS = _jwsGenerator.Build(payload.ToString(), "PS384", rsaJsonWebKey);
            var ps512JWS = _jwsGenerator.Build(payload.ToString(), "PS512", rsaJsonWebKey);
            var isRSA256JWSValid = _jwsGenerator.Check(rsa256JWS, rsaJsonWebKey);
            var isRSA384JWSValid = _jwsGenerator.Check(rsa384JWS, rsaJsonWebKey);
            var isRSA512JWSValid = _jwsGenerator.Check(rsa512JWS, rsaJsonWebKey);
            var isEC256JWSValid = _jwsGenerator.Check(ec256JWS, ecJsonWebKey);
            var isEC384JWSValid = _jwsGenerator.Check(ec384JWS, ecJsonWebKey);
            var isEC512JWSValid = _jwsGenerator.Check(ec512JWS, ecJsonWebKey);
            var isHS256JWSValid = _jwsGenerator.Check(hs256JWS, hmac256JsonWebKey);
            var isHS384JWSValid = _jwsGenerator.Check(hs384JWS, hmac256JsonWebKey);
            var isHS512JWSValid = _jwsGenerator.Check(hs512JWS, hmac256JsonWebKey);
            var isPS256JWSValid = _jwsGenerator.Check(ps256JWS, rsaJsonWebKey);
            var isPS384JWSValid = _jwsGenerator.Check(ps384JWS, rsaJsonWebKey);
            var isPS512JWSValid = _jwsGenerator.Check(ps512JWS, rsaJsonWebKey);


            // ASSERT
            Assert.True(isRSA256JWSValid);
            Assert.True(isRSA384JWSValid);
            Assert.True(isRSA512JWSValid);
            Assert.True(isEC256JWSValid);
            Assert.True(isEC384JWSValid);
            Assert.True(isEC512JWSValid);
            Assert.True(isHS256JWSValid);
            Assert.True(isHS384JWSValid);
            Assert.True(isHS512JWSValid);
            Assert.True(isPS256JWSValid);
            Assert.True(isPS384JWSValid);
            Assert.True(isPS512JWSValid);
        }

        private static void InitializeFakeObjects()
        {
            _jwsGenerator = new JwsGenerator(new ISignHandler[]
            {
                new ECDSAP256SignHandler(),
                new ECDSAP384SignHandler(),
                new ECDSAP512SignHandler(),
                new RSA256SignHandler(),
                new RSA384SignHandler(),
                new RSA512SignHandler(),
                new HMAC256SignHandler(),
                new HMAC384SignHandler(),
                new HMAC512SignHandler(),
                new PS256SignHandler(),
                new PS384SignHandler(),
                new PS512SignHandler()
            });
        }
    }
}
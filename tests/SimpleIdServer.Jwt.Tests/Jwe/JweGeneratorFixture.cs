// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Jwe;
using SimpleIdServer.Jwt.Jwe.CEKHandlers;
using SimpleIdServer.Jwt.Jwe.EncHandlers;
using System.Security.Cryptography;
using Xunit;

namespace SimpleIdServer.Jwt.Tests.Jwe
{
    public class JweGeneratorFixture
    {
        private static IJweGenerator _jweGenerator;

        [Fact]
        public void When_Build_Jwe_Then_Can_Decrypt_Into_Jws()
        {
            // https://github.com/dvsekhvalnov/jose-jwt/search?q=A192CBC_HS384&unscoped_q=A192CBC_HS384
            const string payload = "xml";
            // ARRANGE
            InitializeFakeObjects();
            JsonWebKey rsaJsonWebKey;
            using (var rsa = RSA.Create())
            {
                rsaJsonWebKey = new JsonWebKeyBuilder().NewSign("keyId", new[]
                {
                    KeyOperations.Encrypt
                }).SetAlg(rsa, RSAOAEPCEKHandler.ALG_NAME).Build();
            }            

            // ACT
            var encrypted = _jweGenerator.Build(payload, RSAOAEPCEKHandler.ALG_NAME, A192CBCHS384EncHandler.ENC_NAME, rsaJsonWebKey);
            var decrypted = _jweGenerator.Decrypt(encrypted, rsaJsonWebKey);

            // ASSERT
            Assert.Equal(payload, decrypted);
        }

        private void InitializeFakeObjects()
        {
            _jweGenerator = new JweGenerator(new IEncHandler[]
            {
                new A128CBCHS256EncHandler(),
                new A192CBCHS384EncHandler(),
                new A256CBCHS512EncHandler(),
            }, new ICEKHandler[]
            {
                new RSA15CEKHandler(),
                new RSAOAEP256CEKHandler(),
                new RSAOAEPCEKHandler()
            });
        }
    }
}

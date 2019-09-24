// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt;
using SimpleIdServer.Jwt.Jws.Handlers;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SimpleIdServer.OAuth.Host.Acceptance.Tests
{
    public class FakeJwks
    {
        private IEnumerable<JsonWebKey> _jsonWebKeys;
        private static FakeJwks _instance;

        private FakeJwks()
        {
            Init();
        }

        public static FakeJwks GetInstance()
        {
            if (_instance == null)
            {
                _instance = new FakeJwks();
            }

            return _instance;
        }

        public IEnumerable<JsonWebKey> Jwks => _jsonWebKeys;

        private void Init()
        {
            var builder = new JsonWebKeyBuilder();
            using (var rsa = RSA.Create())
            {
                var sig = builder.NewSign("1", new[]
                {
                    KeyOperations.Sign,
                    KeyOperations.Verify
                }).SetAlg(rsa, RSA256SignHandler.ALG_NAME).Build();
                _jsonWebKeys = new[]
                {
                    sig
                };
            }
        }
    }
}

// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Extensions;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SimpleIdServer.Jwt
{
    public class JsonWebKeyEncBuilder
    {
        private readonly JsonWebKey _jsonWebKey;

        internal JsonWebKeyEncBuilder(JsonWebKey jsonWebKey)
        {
            _jsonWebKey = jsonWebKey;
        }

        public JsonWebKeyEncBuilder SetAlg(ECDsaCng ec, string algName)
        {
            _jsonWebKey.Alg = algName;
            _jsonWebKey.Kty = KeyTypes.EC;
            foreach (var kvp in ec.ExtractPublicKey())
            {
                _jsonWebKey.Content.Add(kvp.Key, kvp.Value);
            }

            foreach (var kvp in ec.ExtractPrivateKey())
            {
                _jsonWebKey.Content.Add(kvp.Key, kvp.Value);
            }

            return this;
        }

        public JsonWebKeyEncBuilder SetAlg(RSA rsa, string algName)
        {
            _jsonWebKey.Alg = algName;
            _jsonWebKey.Kty = KeyTypes.RSA;
            foreach(var kvp in rsa.ExtractPublicKey())
            {
                _jsonWebKey.Content.Add(kvp.Key, kvp.Value);
            }

            foreach (var kvp in rsa.ExtractPrivateKey())
            {
                _jsonWebKey.Content.Add(kvp.Key, kvp.Value);
            }
            return this;
        }

        public JsonWebKey Build()
        {
            return _jsonWebKey;
        }
    }

    public class JsonWebKeySignBuilder
    {
        private readonly JsonWebKey _jsonWebKey;

        internal JsonWebKeySignBuilder(JsonWebKey jsonWebKey)
        {
            _jsonWebKey = jsonWebKey;
        }

        public JsonWebKeySignBuilder SetAlg(HMAC hmac, string algName)
        {
            _jsonWebKey.Alg = algName;
            _jsonWebKey.Kty = KeyTypes.OCT;
            foreach (var kvp in hmac.ExportKey())
            {
                _jsonWebKey.Content.Add(kvp.Key, kvp.Value);
            }

            return this;
        }

        public JsonWebKeySignBuilder SetAlg(ECDsaCng ec, string algName)
        {
            _jsonWebKey.Alg = algName;
            _jsonWebKey.Kty = KeyTypes.EC;
            foreach (var kvp in ec.ExtractPublicKey())
            {
                _jsonWebKey.Content.Add(kvp.Key, kvp.Value);
            }

            foreach (var kvp in ec.ExtractPrivateKey())
            {
                _jsonWebKey.Content.Add(kvp.Key, kvp.Value);
            }

            return this;
        }

        public JsonWebKeySignBuilder SetAlg(RSA rsa, string algName)
        {
            _jsonWebKey.Alg = algName;
            _jsonWebKey.Kty = KeyTypes.RSA;
            foreach (var kvp in rsa.ExtractPublicKey())
            {
                _jsonWebKey.Content.Add(kvp.Key, kvp.Value);
            }

            foreach (var kvp in rsa.ExtractPrivateKey())
            {
                _jsonWebKey.Content.Add(kvp.Key, kvp.Value);
            }

            return this;
        }

        public JsonWebKey Build()
        {
            return _jsonWebKey;
        }
    }

    public class JsonWebKeyBuilder
    {
        public JsonWebKeyEncBuilder NewEnc(string keyId, ICollection<KeyOperations> keyOps)
        {
            var jsonWebKey = new JsonWebKey(keyId, Usages.ENC, keyOps);
            return new JsonWebKeyEncBuilder(jsonWebKey);
        }

        public JsonWebKeySignBuilder NewSign(string keyId, ICollection<KeyOperations> keyOps)
        {
            var jsonWebKey = new JsonWebKey(keyId, Usages.SIG, keyOps);
            return new JsonWebKeySignBuilder(jsonWebKey);
        }
    }
}

// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Stores;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Api.Jwks
{
    public interface IJwksRequestHandler
    {
        JwksResult Get();
    }

    public class JwksRequestHandler : IJwksRequestHandler
    {
        private readonly IKeyStore _keyStore;

        public JwksRequestHandler(IKeyStore keyStore)
        {
            _keyStore = keyStore;
        }

        public JwksResult Get()
        {
            var result = new JwksResult();
            var signingKeys = _keyStore.GetAllSigningKeys();
            var encKeys = _keyStore.GetAllEncryptingKeys();
            // JWT signing keys : Public key pairs for signing issued JWTs that are access tokens, ID Tokens etc... Clients can download them to validate the JWTs.
            foreach(var key in signingKeys)
                result.JsonWebKeys.Add(ConvertSigningKey(key));

            // JWT encryption keys : Public keys for letting clients encrypt JWTs that are request objects.
            foreach (var key in encKeys)
                result.JsonWebKeys.Add(ConvertEncryptionKey(key));

            return result;

            JsonObject ConvertSigningKey(SigningCredentials signingCredentials)
            {
                var publicJwk = signingCredentials.SerializePublicJWK();
                return JsonNode.Parse(JsonExtensions.SerializeToJson(publicJwk)).AsObject();
            }

            JsonObject ConvertEncryptionKey(EncryptingCredentials encryptingCredentials)
            {
                var publicJwk = encryptingCredentials.SerializePublicJWK();
                return JsonNode.Parse(JsonExtensions.SerializeToJson(publicJwk)).AsObject();
            }
        }
    }
}

// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.OAuth.Stores;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Nodes;

namespace SimpleIdServer.OAuth.Api.Jwks
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
            foreach(var key in signingKeys)
                result.JsonWebKeys.Add(Convert(key));

            foreach (var key in encKeys)
                result.JsonWebKeys.Add(Convert(key));

            return result;

            JsonObject Convert(SigningCredentials signingCredentials)
            {
                var publicJwk = signingCredentials.SerializePublicJWK();
                return JsonNode.Parse(JsonExtensions.SerializeToJson(publicJwk)).AsObject();
            }

            JsonObject Convert(EncryptingCredentials encryptingCredentials)
            {
                var publicJwk = encryptingCredentials.SerializePublicJWK();
                return JsonNode.Parse(JsonExtensions.SerializeToJson(publicJwk)).AsObject();
            }
        }
    }
}

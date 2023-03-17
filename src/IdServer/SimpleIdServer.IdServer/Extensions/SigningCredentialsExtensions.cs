// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text.Json.Nodes;

namespace Microsoft.IdentityModel.Tokens
{
    public static class SigningCredentialsExtensions
    {
        public static JsonWebKey SerializePublicJWK(this SigningCredentials credentials)
        {
            if(credentials.Key is RsaSecurityKey rsa)
            {
                var parameters = rsa.Rsa.ExportParameters(false);
                credentials = new SigningCredentials(new RsaSecurityKey(parameters) { KeyId = credentials.Kid }, credentials.Algorithm);
            }

            if (credentials.Key is ECDsaSecurityKey ecdsa)
            {
                var parameters = ecdsa.ECDsa.ExportParameters(false);
                credentials = new SigningCredentials(new ECDsaSecurityKey(ECDsa.Create(parameters)) { KeyId = credentials.Kid }, credentials.Algorithm);
            }

            var result = SerializeJWK(credentials);
            return result;
        }

        public static string SerializeJWKStr(this SigningCredentials credentials)
        {
            var jsonWebKey = SerializeJWK(credentials);
            return JsonNode.Parse(JsonExtensions.SerializeToJson(jsonWebKey)).AsObject().ToJsonString();
        }

        public static JsonWebKey SerializeJWK(this SigningCredentials credentials)
        {
            var result = JsonWebKeyConverter.ConvertFromSecurityKey(credentials.Key);
            result.Use = Constants.JWKUsages.Sig;
            result.Alg = credentials.Algorithm;
            if (credentials.Key is X509SecurityKey)
            {
                var securityKey = credentials.Key as X509SecurityKey;
                var rsaKey = securityKey.Certificate.PublicKey.GetRSAPublicKey();
                if (rsaKey != null)
                {
                    var parameters = rsaKey.ExportParameters(false);
                    result.E = Base64UrlEncoder.Encode(parameters.Exponent);
                    result.N = Base64UrlEncoder.Encode(parameters.Modulus);
                }
            }

            return result;
        }
    }
}

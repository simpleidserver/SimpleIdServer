// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.DPoP;
using SimpleIdServer.IdServer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text.Json.Nodes;

namespace Microsoft.IdentityModel.Tokens
{
    public static class EncryptingCredentialsExtensions
    {
        public static JsonWebKey SerializePublicJWK(this EncryptingCredentials credentials)
        {
            if (credentials.Key is RsaSecurityKey rsa)
            {
                var parameters = rsa.Rsa.ExportParameters(false);
                credentials = new EncryptingCredentials(new RsaSecurityKey(parameters) { KeyId = credentials.Key.KeyId }, credentials.Alg, credentials.Enc);
            }

            if (credentials.Key is ECDsaSecurityKey ecdsa)
            {
                var parameters = ecdsa.ECDsa.ExportParameters(false);
                credentials = new EncryptingCredentials(new ECDsaSecurityKey(ECDsa.Create(parameters)) { KeyId = credentials.Key.KeyId }, credentials.Alg, credentials.Enc);
            }

            var result = SerializeJWK(credentials);
            return result;
        }

        public static string SerializeJWKStr(this EncryptingCredentials credentials)
        {
            var jsonWebKey = SerializeJWK(credentials);
            return JsonNode.Parse(JsonWebKeySerializer.Write(jsonWebKey)).AsObject().ToJsonString();
        }

        public static JsonWebKey SerializeJWK(this EncryptingCredentials credentials)
        {
            var result = JsonWebKeyConverter.ConvertFromSecurityKey(credentials.Key);
            result.Use = Constants.JWKUsages.Enc;
            result.Alg = credentials.Alg;
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

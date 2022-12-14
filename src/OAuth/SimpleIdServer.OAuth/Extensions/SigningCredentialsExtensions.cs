// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Microsoft.IdentityModel.Tokens
{
    public static class SigningCredentialsExtensions
    {
        public static JsonWebKey SerializePublicJWK(this SigningCredentials credentials)
        {
            switch(credentials.Key)
            {
                case RsaSecurityKey rsa:
                    return rsa.SerializePublicJWK(credentials.Algorithm);
            }

            return null;
        }

        public static JsonWebKey SerializePublicJWK(this RsaSecurityKey rsa, string alg)
        {
            var parameters = rsa.Rsa?.ExportParameters(false) ?? rsa.Parameters;
            var exponent = Base64UrlEncoder.Encode(parameters.Exponent);
            var modulus = Base64UrlEncoder.Encode(parameters.Modulus);
            return new JsonWebKey
            {
                Kty = "RSA",
                Use = "sig",
                Kid = rsa.KeyId,
                E = exponent,
                N = modulus,
                Alg = alg
            };
        }
    }
}

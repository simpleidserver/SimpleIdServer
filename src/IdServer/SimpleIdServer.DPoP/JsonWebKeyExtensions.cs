// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SimpleIdServer.DPoP
{
    public static class JsonWebKeyExtensions
    {
        /// <summary>
        /// The value of the HTTP method of the request to which the JWT is attached.
        /// </summary>
        /// <param name="jsonWebToken"></param>
        /// <returns></returns>
        public static string Htm(this JsonWebToken jsonWebToken)
        {
            if (jsonWebToken.TryGetClaim(DPoPConstants.DPoPClaims.Htm, out Claim cl)) return cl.Value;
            return null;
        }

        /// <summary>
        /// The HTTP Target URI, without query and fragment parts, of tyhe request to which the JWT is attached.
        /// </summary>
        /// <param name="jsonWebToken"></param>
        /// <returns></returns>
        public static string Htu(this JsonWebToken jsonWebToken)
        {
            if (jsonWebToken.TryGetClaim(DPoPConstants.DPoPClaims.Htu, out Claim cl)) return cl.Value;
            return null;
        }

        /// <summary>
        /// Nonce
        /// </summary>
        /// <param name="jsonWebToken"></param>
        /// <returns></returns>
        public static string Nonce(this JsonWebToken jsonWebToken)
        {
            if (jsonWebToken.TryGetClaim(DPoPConstants.DPoPClaims.Nonce, out Claim cl)) return cl.Value;
            return null;
        }

        /// <summary>
        /// Public Key.
        /// </summary>
        /// <param name="jsonWebToken"></param>
        /// <returns></returns>
        public static JsonWebKey PublicKey(this JsonWebToken jsonWebToken)
        {
            string jwkJson;
            if (!jsonWebToken.TryGetHeaderValue(DPoPConstants.Jwk, out jwkJson)) return null;
            return JsonExtensions.DeserializeFromJson<JsonWebKey>(jwkJson);
        }

        public static string CreateThumbprint(this JsonWebKey jwk)
        {
            var jkt = Base64UrlEncoder.Encode(jwk.ComputeJwkThumbprint());
            return jkt;
        }
    }
}

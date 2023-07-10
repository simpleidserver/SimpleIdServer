// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
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
    }
}

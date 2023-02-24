// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer;

namespace Microsoft.IdentityModel.Tokens
{
    public static class SigningCredentialsExtensions
    {
        public static JsonWebKey SerializePublicJWK(this SigningCredentials credentials)
        {
            var result = JsonWebKeyConverter.ConvertFromSecurityKey(credentials.Key);
            result.Use = Constants.JWKUsages.Sig;
            result.Alg = credentials.Algorithm;
            return result;
        }
    }
}

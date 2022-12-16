// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Nodes;

namespace Microsoft.IdentityModel.JsonWebTokens
{
    public static class JsonWebTokenExtensions
    {
        public static JsonObject GetClaimJson(this JsonWebToken jws)
        {
            if (jws.Claims == null) return null;
            var result = new JsonObject();
            foreach(var claim in jws.Claims) { result.Add(claim.Type, claim.Value); }
            return result;
        }
    }
}

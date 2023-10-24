// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Microsoft.IdentityModel.JsonWebTokens
{
    public static class JsonWebTokenExtensions
    {
        public static JsonObject GetClaimJson(this JsonWebToken jws)
        {
            if (jws.Claims == null) return null;
            var result = new JsonObject();
            foreach(var claim in jws.Claims) 
            {
                try
                {
                    result.Add(claim.Type, JsonNode.Parse(claim.Value));
                }
                catch
                {
                    result.Add(claim.Type, claim.Value);
                }

            }
            return result;
        }


        public static Dictionary<string, object> GetClaimsDic(this JsonWebToken jws)
        {
            var result = new Dictionary<string, object>();
            foreach (var kvp in jws.Claims.GroupBy(c => c.Type))
            {
                if (kvp.Count() > 1) result.Add(kvp.Key, kvp.Select(k => k.Value));
                else result.Add(kvp.Key, kvp.Single().Value);
            }

            return result;
        }
    }
}

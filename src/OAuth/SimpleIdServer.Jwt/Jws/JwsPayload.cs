// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace SimpleIdServer.Jwt.Jws
{
    /// <summary>
    /// Represents a JSON Web Token
    /// </summary>
    [KnownType(typeof(object[]))]
    [KnownType(typeof(string[]))]
    public class JwsPayload : Dictionary<string, object>
    {
        public string GetClaimValue(string claimName)
        {
            if (!ContainsKey(claimName) || this[claimName] == null)
            {
                return null;
            }

            return this[claimName].ToString();
        }

        private string GetStringClaim(string claimName)
        {
            if (!ContainsKey(claimName))
            {
                return null;
            }

            return this[claimName].ToString();
        }

        public double GetDoubleClaim(string claimName)
        {
            if (!ContainsKey(claimName))
            {
                return default(double);
            }

            double result;
            var claim = this[claimName].ToString();
            if (double.TryParse(claim, out result))
            {
                return result;
            }

            return default(double);
        }

        public string[] GetArrayClaim(string claimName)
        {
            if (!ContainsKey(claimName))
            {
                return new string[0];
            }

            var claim = this[claimName];
            var arr = claim as object[];
            var jArr = claim as JArray;
            if (arr != null)
            {
                return arr.Select(c => c.ToString()).ToArray();
            }

            if (jArr != null)
            {
                return jArr.Select(c => c.ToString()).ToArray();
            }

            return new[] { claim.ToString() };
        }

        public bool TryAdd(string key, object value)
        {
            if (ContainsKey(key))
            {
                return false;
            }

            Add(key, value);
            return true;
        }

        public JwsPayload Copy()
        {
            var result = new JwsPayload();
            foreach(var kvp in this)
            {
                result.Add(kvp.Key, kvp.Value);
            }

            return result;
        }
    }
}

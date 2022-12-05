// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text.Json.Nodes;

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
            var jArr = (claim as JsonNode)?.AsArray();
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

        public void AddOrReplace(IEnumerable<Claim> claims)
        {
            foreach(var claim in claims)
            {
                AddOrReplace(claim);
            }
        }

        public void AddOrReplace(Claim claim)
        {
            var value = ExtractValue(claim);
            if (value == null)
            {
                return;
            }

            if (ContainsKey(claim.Type))
            {
                var lst = typeof(List<>).MakeGenericType(value.GetType());
                var lstValues = this[claim.Type];
                if (lstValues.GetType() != lst)
                {
                    lstValues = Activator.CreateInstance(lst);
                    var existingValue = this[claim.Type];
                    lst.GetMethod("Add").Invoke(lstValues, new[] { existingValue });
                    this[claim.Type] = lstValues;
                }

                lst.GetMethod("Add").Invoke(lstValues, new[] { value });
            }
            else
            {
                Add(claim.Type, value);
            }
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

        private static object ExtractValue(Claim claim)
        {
            switch(claim.ValueType)
            {
                case ClaimValueTypes.BOOLEAN:
                    {
                        if (bool.TryParse(claim.Value, out bool r))
                        {
                            return r;
                        }
                    }
                    break;
                case ClaimValueTypes.INTEGER:
                    {
                        if (int.TryParse(claim.Value, out int r))
                        {
                            return r;
                        }
                    }
                    break;
                case ClaimValueTypes.JSONOBJECT:
                    {
                        if (!string.IsNullOrWhiteSpace(claim.Value))
                        {
                            var jObj = JsonObject.Parse(claim.Value);
                            if (jObj != null)
                            {
                                return jObj;
                            }
                        }
                    }
                    break;
                default:
                    return claim.Value;
            }

            return null;
        }
    }
}

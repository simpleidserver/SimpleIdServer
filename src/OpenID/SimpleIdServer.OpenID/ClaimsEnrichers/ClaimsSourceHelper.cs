// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Jws;
using System.Collections.Generic;

namespace SimpleIdServer.OpenID.ClaimsEnrichers
{
    public static class ClaimsSourceHelper
    {
        private const string CLAIM_NAMES = "_claim_names";
        private const string CLAIM_SOURCES = "_claim_sources";

        public static void AddAggregate(JwsPayload payload, JwsPayload resultPayload, string jwt, string name)
        {
            Init(payload);
            var claimNames = payload[CLAIM_NAMES] as JObject;
            var claimSources = payload[CLAIM_SOURCES] as JObject;
            foreach(var kvp in resultPayload)
            {
                claimNames.Add(kvp.Key, name);
            }

            var record = new JObject
            {
                { "JWT", jwt }
            };
            claimSources.Add(name, record);
        }

        public static void AddDistribute(JwsPayload payload, string name, string url, string apiToken, IEnumerable<string> claims)
        {
            Init(payload);
            var claimNames = payload[CLAIM_NAMES] as JObject;
            var claimSources = payload[CLAIM_SOURCES] as JObject;
            foreach (var claim in claims)
            {
                claimNames.Add(claim, name);
            }

            var record = new JObject
            {
                { "endpoint", url },
                { "access_token", apiToken }
            };
            claimSources.Add(name, record);
        }

        private static void Init(JwsPayload payload)
        {
            if (!payload.ContainsKey(CLAIM_NAMES))
            {
                payload.Add(CLAIM_NAMES, new JObject());
            }

            if (!payload.ContainsKey(CLAIM_SOURCES))
            {
                payload.Add(CLAIM_SOURCES, new JObject());
            }
        }
    }
}

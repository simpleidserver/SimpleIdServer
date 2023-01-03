// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace SimpleIdServer.OpenID.ClaimsEnrichers
{
    public static class ClaimsSourceHelper
    {
        private const string CLAIM_NAMES = "_claim_names";
        private const string CLAIM_SOURCES = "_claim_sources";

        public static void AddAggregate(Dictionary<string, object> payload, JsonObject resultPayload, string jwt, string name)
        {
            Init(payload);
            var claimNames = payload[CLAIM_NAMES] as JsonObject;
            var claimSources = payload[CLAIM_SOURCES] as JsonObject;
            foreach(var kvp in resultPayload)
                claimNames.Add(kvp.Key, name);

            var record = new JsonObject
            {
                ["JWT"] = jwt
            };
            claimSources.Add(name, record);
        }

        public static void AddDistribute(Dictionary<string, object> payload, string name, string url, string apiToken, IEnumerable<string> claims)
        {
            Init(payload);
            var claimNames = payload[CLAIM_NAMES] as JsonObject;
            var claimSources = payload[CLAIM_SOURCES] as JsonObject;
            foreach (var claim in claims)
            {
                claimNames.Add(claim, name);
            }

            var record = new JsonObject
            {
                ["endpoint"] = url,
                ["access_token"] = apiToken
            };
            claimSources.Add(name, record);
        }

        private static void Init(Dictionary<string, object> payload)
        {
            if (!payload.ContainsKey(CLAIM_NAMES))
                payload.Add(CLAIM_NAMES, new JsonObject());

            if (!payload.ContainsKey(CLAIM_SOURCES))
                payload.Add(CLAIM_SOURCES, new JsonObject());
        }
    }
}

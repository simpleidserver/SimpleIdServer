// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.OpenID.ClaimsEnrichers
{
    public class DistributeHttpClaimsSourceOptions
    {
        public DistributeHttpClaimsSourceOptions(string name, string url, string apiToken, IEnumerable<string> claimNames)
        {
            Name = name;
            Url = url;
            ApiToken = apiToken;
            ClaimNames = claimNames;
        }

        public string Name { get; private set; }
        public string Url { get; private set; }
        public string ApiToken { get; private set; }
        public IEnumerable<string> ClaimNames { get; private set; }
    }
}
// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.ClaimsEnrichers
{
    public class DistributeHttpClaimsSource : IClaimsSource
    {
        private readonly DistributeHttpClaimsSourceOptions _distributeHttpClaimsSourceOptions;

        public DistributeHttpClaimsSource(DistributeHttpClaimsSourceOptions distributeHttpClaimsSourceOptions)
        {
            _distributeHttpClaimsSourceOptions = distributeHttpClaimsSourceOptions;
        }

        public Task Enrich(Dictionary<string, object> claims, Client client, CancellationToken cancellationToken)
        {
            ClaimsSourceHelper.AddDistribute(claims, _distributeHttpClaimsSourceOptions.Name, _distributeHttpClaimsSourceOptions.Url, _distributeHttpClaimsSourceOptions.ApiToken, _distributeHttpClaimsSourceOptions.ClaimNames);
            return Task.FromResult(0);
        }
    }

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

// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.ClaimsEnricher
{
    public interface IRelayClaimsExtractor
    {
        Task<AggregatedClaimsExtractionResult> ExtractAggregatedClaims(User user, string connectionString, CancellationToken cancellationToken);
        Task<DistributedClaimsExtractionResult> ExtractDistributedClaims(User user, string connectionString, CancellationToken cancellationToken);
        string ProviderType { get; }
    }

    public class AggregatedClaimsExtractionResult
    {
        public IEnumerable<string> ClaimNames { get; set; }
        public string Jwt { get; set; }
    }

    public class DistributedClaimsExtractionResult
    {
        public IEnumerable<string> ClaimNames { get; set; }
        public string Endpoint { get; set; }
        public string AccessToken { get; set; }
    }
}

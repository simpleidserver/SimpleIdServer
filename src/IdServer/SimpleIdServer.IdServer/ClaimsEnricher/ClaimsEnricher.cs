// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.ClaimsEnricher
{
    public interface IClaimsEnricher
    {
        Task Enrich(User user, Dictionary<string, object> claims, Client client, CancellationToken cancellationToken);
    }

    public class ClaimsEnricher : IClaimsEnricher
    {
        private const string CLAIM_NAMES = "_claim_names";
        private const string CLAIM_SOURCES = "_claim_sources";
        private readonly IClaimProviderRepository _repository;
        private readonly IEnumerable<IRelayClaimsExtractor> _claimsExtractorLst;

        public ClaimsEnricher(IClaimProviderRepository repository, IEnumerable<IRelayClaimsExtractor> claimsSourceLst)
        {
            _repository = repository;
            _claimsExtractorLst = claimsSourceLst;
        }

        public async Task Enrich(User user, Dictionary<string, object> claims, Client client, CancellationToken cancellationToken)
        {
            var claimProviders = await _repository.GetAll(cancellationToken);
            int i = 1;
            foreach(var claimProvider in claimProviders) 
            {
                var claimsExtractor = _claimsExtractorLst.FirstOrDefault(c => c.ProviderType == claimProvider.ProviderType);
                if (claimsExtractor == null) continue;
                switch(claimProvider.ClaimType)
                {
                    case ClaimType.DISTRIBUTED:
                        {
                            var result = await claimsExtractor.ExtractDistributedClaims(user, claimProvider.ConnectionString, cancellationToken);
                            if (result == null) continue;
                            AddDistributedClaims(claims, result, i);
                            i++;
                            break;
                        }
                    case ClaimType.AGGREGATED:
                        {
                            var result = await claimsExtractor.ExtractAggregatedClaims(user, claimProvider.ConnectionString, cancellationToken);
                            if (result == null) continue;
                            AddAggregatedClaims(claims, result, i);
                            i++;
                            break;
                        }
                }
            }

            void AddAggregatedClaims(Dictionary<string, object> claims, AggregatedClaimsExtractionResult result, int i)
            {
                Init(claims);
                var source = $"src{i}";
                var claimNames = claims[CLAIM_NAMES] as JsonObject;
                var claimSources = claims[CLAIM_SOURCES] as JsonObject;
                foreach (var name in result.ClaimNames)
                    claimNames.Add(name, source);

                var record = new JsonObject
                {
                    ["JWT"] = result.Jwt
                };
                claimSources.Add(source, record);
            }

            void AddDistributedClaims(Dictionary<string, object> payload, DistributedClaimsExtractionResult result, int i)
            {
                Init(payload);
                var source = $"src{i}";
                var claimNames = payload[CLAIM_NAMES] as JsonObject;
                var claimSources = payload[CLAIM_SOURCES] as JsonObject;
                foreach (var name in result.ClaimNames)
                    claimNames.Add(name, source);

                var record = new JsonObject
                {
                    ["endpoint"] = result.Endpoint
                };
                if (!string.IsNullOrWhiteSpace(result.AccessToken)) record.Add("access_token", result.AccessToken);
                claimSources.Add(source, record);
            }

            void Init(Dictionary<string, object> claims)
            {
                if (!claims.ContainsKey(CLAIM_NAMES))
                    claims.Add(CLAIM_NAMES, new JsonObject());

                if (!claims.ContainsKey(CLAIM_SOURCES))
                    claims.Add(CLAIM_SOURCES, new JsonObject());
            }
        }
    }
}

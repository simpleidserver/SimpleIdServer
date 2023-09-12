// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using BlushingPenguin.JsonPath;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Jobs;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.Scim.Client;
using SimpleIdServer.Scim.Client.DTOs;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Provisioning.SCIM.Jobs
{
    public class SCIMRepresentationsExtractionJob : RepresentationExtractionJob<SCIMRepresentationsExtractionJobOptions>
    {
        public const string NAME = "SCIM";

        public SCIMRepresentationsExtractionJob(IConfiguration configuration, ILogger<RepresentationExtractionJob<SCIMRepresentationsExtractionJobOptions>> logger, IBusControl busControl, IIdentityProvisioningStore identityProvisioningStore, IExtractedRepresentationRepository extractedRepresentationRepository, IOptions<IdServerHostOptions> options) : base(configuration, logger, busControl, identityProvisioningStore, extractedRepresentationRepository, options)
        {
        }

        public override string Name => NAME;

        protected override async IAsyncEnumerable<List<ExtractedRepresentation>> FetchUsers(SCIMRepresentationsExtractionJobOptions options, string destinationFolder, IdentityProvisioning identityProvisioning)
        {
            using (var scimClient = new SCIMClient(options.SCIMEdp))
            {

                var accessToken = await GetAccessToken(options);
                var searchUsers = await scimClient.SearchUsers(new SearchRequest
                {
                    Count = options.Count,
                    StartIndex = 1
                }, accessToken, CancellationToken.None);
                var filterUsers = await FilterUsers(searchUsers.Item1);
                ExtractUsers(filterUsers, 1, destinationFolder, identityProvisioning.Definition);
                yield return filterUsers.Select(r => new ExtractedRepresentation
                {
                    ExternalId = r.Id,
                    Version = r.Meta.Version.ToString()
                }).ToList();
                var totalResults = searchUsers.Item1.TotalResults;
                var count = searchUsers.Item1.ItemsPerPage;
                var nbPages = ((int)Math.Ceiling((double)totalResults / count));
                var allPages = Enumerable.Range(2, nbPages - 1);
                foreach(var currentPage in allPages)
                {
                    var newSearchUsers = await scimClient.SearchUsers(new SearchRequest
                    {
                        Count = count,
                        StartIndex = currentPage * count
                    }, accessToken, CancellationToken.None);
                    var newFilterUsers = FilterUsers(newSearchUsers.Item1).Result;
                    ExtractUsers(newFilterUsers, currentPage, destinationFolder, identityProvisioning.Definition);
                    yield return newFilterUsers.Select(r => new ExtractedRepresentation
                    {
                        ExternalId = r.Id,
                        Version = r.Meta.Version.ToString()
                    }).ToList();
                }
            }
        }

        private async Task<IEnumerable<RepresentationResult>> FilterUsers(SearchResult<RepresentationResult> searchResult)
        {
            var ids = searchResult.Resources.Select(r => r.Id);
            var extractedRepresentations = await ExtractedRepresentationRepository.BulkRead(ids);
            return searchResult.Resources.Where((r) =>
            {
                var er = extractedRepresentations.SingleOrDefault(er => er.ExternalId == r.Id);
                return er == null || er.Version != r.Meta.Version.ToString();
            }).ToList();
        }

        private Task<string> GetAccessToken(SCIMRepresentationsExtractionJobOptions options)
        {
            switch(options.AuthenticationType)
            {
                case ClientAuthenticationTypes.APIKEY:
                    return Task.FromResult(options.ApiKey);
                default:
                    throw new NotImplementedException($"Authentication {options.AuthenticationType} is not supported");
            }
        }

        private void ExtractUsers(IEnumerable<RepresentationResult> resources, int currentPage, string destinationFolder, IdentityProvisioningDefinition definition)
        {
            if (!resources.Any()) return;
            using (var fs = File.CreateText(Path.Combine(destinationFolder, $"{currentPage}.csv")))
            {
                fs.WriteLine(BuildFileColumns(definition));
                foreach (var resource in resources)
                    fs.WriteLine($"{resource.Id}{Constants.IdProviderSeparator}{resource.Meta.Version}{Constants.IdProviderSeparator}{Extract(resource, definition)}");
            }
        }

        private string Extract(RepresentationResult result, IdentityProvisioningDefinition provisioningDefinition)
        {
            var jsonDoc = JsonDocument.Parse(result.AdditionalData.ToJsonString());
            var values = new List<string>();
            foreach (var mappingRule in provisioningDefinition.MappingRules)
            {
                var token = jsonDoc.SelectToken(mappingRule.From);
                if (token == null)
                {
                    values.Add(string.Empty);
                    continue;
                }

                values.Add(token.Value.GetString());
            }

            return string.Join(Constants.IdProviderSeparator, values);
        }
    }
}

// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using BlushingPenguin.JsonPath;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.Scim.Client;
using SimpleIdServer.Scim.Client.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace SimpleIdServer.IdServer.Jobs
{
    public class SCIMRepresentationsExtractionJob : RepresentationExtractionJob
    {
        private readonly IIdentityProvisioningStore _identityProvisioningStore;
        private readonly IBusControl _busControl;
        private readonly IExtractedRepresentationRepository _extractedRepresentationRepository;
        private readonly IdServerHostOptions _options;
        private readonly ILogger<SCIMRepresentationsExtractionJob> _logger;

        public SCIMRepresentationsExtractionJob(IIdentityProvisioningStore identityProvisioningStore, IBusControl busControl, IExtractedRepresentationRepository extractedRepresentationRepository, IOptions<IdServerHostOptions> options, ILogger<SCIMRepresentationsExtractionJob> logger)
        {
            _identityProvisioningStore = identityProvisioningStore;
            _busControl = busControl;
            _extractedRepresentationRepository = extractedRepresentationRepository;
            _options = options.Value;
            _logger = logger;
        }

        public const string NAME = "SCIM";
        public override string Name => NAME;

        public async override Task Execute(string instanceId, string prefix)
        {
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Start to export SCIM users"))
            {
                _logger.LogInformation("Start to export SCIM users");
                var lck = new object();
                var metadata = await _identityProvisioningStore.Query()
                    .Include(d => d.Properties)
                    .Include(d => d.Realms)
                    .Include(d => d.Histories)
                    .Include(d => d.Definition).ThenInclude(d => d.MappingRules)
                    .FirstOrDefaultAsync(i => i.Id == instanceId && i.Realms.Any(r => r.Name == prefix));
                if (!metadata.IsEnabled)
                {
                    _logger.LogInformation($"The job {instanceId} is disabled");
                    return;
                }

                var startDateTime = DateTime.UtcNow;
                try
                {
                    var optionsType = typeof(SCIMRepresentationsExtractionJobOptions);
                    var destinationFolder = Path.Combine(_options.ExtractRepresentationsFolder, Name);
                    if (!Directory.Exists(destinationFolder)) Directory.CreateDirectory(destinationFolder);
                    var folderName = DateTime.Now.ToString("MMddyyyyHHmm");
                    destinationFolder = Path.Combine(destinationFolder, folderName);
                    Directory.CreateDirectory(destinationFolder);
                    var options = Serializer.PropertiesSerializer.DeserializeOptions<SCIMRepresentationsExtractionJobOptions, IdentityProvisioningProperty>(metadata.Properties);
                    int nbUsers = 0;

                    using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        using (var scimClient = new SCIMClient(options.SCIMEdp))
                        {
                            var searchUsers = await scimClient.SearchUsers(new SearchRequest
                            {
                                Count = options.Count,
                                StartIndex = 1
                            }, null, CancellationToken.None);
                            var filterUsers = await FilterUsers(searchUsers.Item1);
                            ExtractUsers(filterUsers, 1, destinationFolder, metadata.Definition);
                            await _extractedRepresentationRepository.BulkUpdate(filterUsers.Select(r => new ExtractedRepresentation
                            {
                                ExternalId = r.Id,
                                Version = r.Meta.Version.ToString()
                            }).ToList());
                            var totalResults = searchUsers.Item1.TotalResults;
                            var count = searchUsers.Item1.ItemsPerPage;
                            nbUsers += filterUsers.Count();
                            var nbPages = ((int)Math.Ceiling((double)totalResults / count));
                            var allPages = Enumerable.Range(2, nbPages - 1);
                            await Parallel.ForEachAsync(allPages, new ParallelOptions
                            {
                                MaxDegreeOfParallelism = 5
                            }, async (currentPage, e) =>
                            {
                                var newSearchUsers = await scimClient.SearchUsers(new SearchRequest
                                {
                                    Count = count,
                                    StartIndex = currentPage * count
                                }, null, CancellationToken.None);
                                lock (lck)
                                {
                                    var newFilterUsers = FilterUsers(newSearchUsers.Item1).Result;
                                    ExtractUsers(newFilterUsers, currentPage, destinationFolder, metadata.Definition);
                                    nbUsers += newFilterUsers.Count();
                                    _extractedRepresentationRepository.BulkUpdate(newFilterUsers.Select(r => new ExtractedRepresentation
                                    {
                                        ExternalId = r.Id,
                                        Version = r.Meta.Version.ToString()
                                    }).ToList()).Wait();
                                }
                            });
                        }

                        _logger.LogInformation($"{nbUsers} users has been exported into {destinationFolder}");
                        activity?.SetStatus(ActivityStatusCode.Ok, $"{nbUsers} users has been exported into {destinationFolder}");
                        var endDateTime = DateTime.UtcNow;
                        metadata.Import(startDateTime, endDateTime, folderName, nbUsers);
                        await _busControl.Publish(new ExtractRepresentationsSuccessEvent
                        {
                            IdentityProvisioningName = NAME,
                            NbRepresentations = nbUsers
                        });
                        transactionScope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    activity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
                    _logger.LogError(ex.ToString());
                    var endDateTime = DateTime.UtcNow;
                    metadata.FailToImport(startDateTime, endDateTime, ex.ToString());
                    await _busControl.Publish(new ExtractRepresentationsFailureEvent
                    {
                        IdentityProvisioningName = NAME,
                        ErrorMessage = ex.ToString()
                    });
                }
                finally
                {
                    await _identityProvisioningStore.SaveChanges(CancellationToken.None);
                }
            }
        }

        private async Task<IEnumerable<RepresentationResult>> FilterUsers(SearchResult<RepresentationResult> searchResult)
        {
            var ids = searchResult.Resources.Select(r => r.Id);
            var extractedRepresentations = await _extractedRepresentationRepository.BulkRead(ids);
            return searchResult.Resources.Where((r) =>
            {
                var er = extractedRepresentations.SingleOrDefault(er => er.ExternalId == r.Id);
                return er == null || er.Version != r.Meta.Version.ToString();
            }).ToList();
        }

        private void ExtractUsers(IEnumerable<RepresentationResult> resources, int currentPage, string destinationFolder, IdentityProvisioningDefinition definition)
        {
            if (!resources.Any()) return;
            using (var fs = File.CreateText(Path.Combine(destinationFolder, $"{currentPage}.csv")))
            {
                fs.WriteLine(BuildFileColumns(definition));
                foreach (var resource in resources)
                    fs.WriteLine($"{resource.Id}{SEPARATOR}{resource.Meta.Version}{SEPARATOR}{Extract(resource, definition)}");
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

            return string.Join(SEPARATOR, values);
        }
    }
}

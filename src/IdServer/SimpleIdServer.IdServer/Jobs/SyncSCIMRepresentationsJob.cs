// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.Scim.Client;
using SimpleIdServer.Scim.Client.DTOs;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Jobs
{
    public class SyncSCIMRepresentationsJob : IdentityProvisioningJob
    {
        private readonly IIdentityProvisioningStore _identityProvisioningStore;
        private readonly ILogger<SyncSCIMRepresentationsJob> _logger;

        public SyncSCIMRepresentationsJob(IIdentityProvisioningStore identityProvisioningStore, ILogger<SyncSCIMRepresentationsJob> logger)
        {
            _identityProvisioningStore = identityProvisioningStore;
            _logger = logger;
        }

        public const string NAME = "SCIM";
        public override string Name => NAME;

        public async override Task Execute(string instanceId, CancellationToken cancellationToken)
        {
            var metadata = await _identityProvisioningStore.Query()
                .Include(d => d.Properties)
                .Include(d => d.Definition).ThenInclude(d => d.MappingRules)
                .FirstOrDefaultAsync(i => i.Name == instanceId);
            if (!metadata.IsEnabled) return;
            var optionsType = typeof(SyncSCIMRepresentationsOptions);
            var destinationFolder = Path.Combine(Directory.GetCurrentDirectory(), Name);
            if (!Directory.Exists(destinationFolder)) Directory.CreateDirectory(destinationFolder);
            destinationFolder = Path.Combine(destinationFolder, DateTime.Now.ToString("MMddyyyyHHmm"));
            Directory.CreateDirectory(destinationFolder);
            var options = Serializer.PropertiesSerializer.DeserializeOptions<SyncSCIMRepresentationsOptions, IdentityProvisioningProperty>(metadata.Properties);
            using (var scimClient = new SCIMClient(options.SCIMEdp))
            {
                var searchUsers = await scimClient.SearchUsers(new SearchRequest
                {
                    Count = options.Count,
                    StartIndex = 1
                }, null, cancellationToken);
                ExtractUsers(searchUsers.Item1, 1, destinationFolder);
                var totalResults = searchUsers.Item1.TotalResults;
                var count = searchUsers.Item1.ItemsPerPage;
                var nbPages = ((int)Math.Ceiling((double)totalResults / count)) - 1;
                var allPages = Enumerable.Range(2, nbPages);
                await Parallel.ForEachAsync(allPages, new ParallelOptions
                {
                    MaxDegreeOfParallelism = 5
                }, async (currentPage, e) =>
                {
                    searchUsers = await scimClient.SearchUsers(new SearchRequest
                    {
                        Count = count,
                        StartIndex = currentPage * count
                    }, null, cancellationToken);
                    ExtractUsers(searchUsers.Item1, currentPage, destinationFolder);
                });
            }
        }

        private void ExtractUsers(SearchResult<RepresentationResult> searchResult, int currentPage, string destinationFolder)
        {
            // Extracts users information by using MAPPING.
            using (var fs = File.CreateText(Path.Combine(destinationFolder, $"{currentPage}.csv")))
            {
                fs.WriteLine("Id;Version;");
                foreach(var resource in searchResult.Resources)
                {
                    fs.WriteLine($"{resource.Id};{resource.Meta.Version};");
                }
            }
        }
    }
}

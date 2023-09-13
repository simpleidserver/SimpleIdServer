// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Jobs;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;

namespace SimpleIdServer.IdServer.Provisioning.SCIM.Jobs
{
    public class SCIMRepresentationsExtractionJob : RepresentationExtractionJob<SCIMRepresentationsExtractionJobOptions>
    {
        public const string NAME = "SCIM";
        private readonly IProvisioningService _provisioningService;

        public SCIMRepresentationsExtractionJob(IEnumerable<IProvisioningService> provisioningServices, IConfiguration configuration, ILogger<RepresentationExtractionJob<SCIMRepresentationsExtractionJobOptions>> logger, IBusControl busControl, IIdentityProvisioningStore identityProvisioningStore, IExtractedRepresentationRepository extractedRepresentationRepository, IOptions<IdServerHostOptions> options) : base(configuration, logger, busControl, identityProvisioningStore, extractedRepresentationRepository, options)
        {
            _provisioningService = provisioningServices.Single(s => s.Name == NAME);
        }

        public override string Name => NAME;

        protected override async IAsyncEnumerable<List<ExtractedRepresentation>> FetchUsers(SCIMRepresentationsExtractionJobOptions options, string destinationFolder, IdentityProvisioning identityProvisioning)
        {
            await foreach (var extractedResult in _provisioningService.Extract(options, identityProvisioning.Definition))
            {
                var result = WriteFile(extractedResult, destinationFolder, identityProvisioning.Definition);
                yield return result;
            }
        }

        private List<ExtractedRepresentation> WriteFile(ExtractedResult extractedResult, string destinationFolder, IdentityProvisioningDefinition definition)
        {
            var result = new List<ExtractedRepresentation>();
            using (var fs = File.CreateText(Path.Combine(destinationFolder, $"{extractedResult.CurrentPage}.csv")))
            {
                fs.WriteLine(BuildFileColumns(definition));
                foreach (var user in extractedResult.Users)
                {
                    fs.WriteLine($"{user.Id}{Constants.IdProviderSeparator}{user.Version}{Constants.IdProviderSeparator}{string.Join(Constants.IdProviderSeparator, user.Values)}");
                    result.Add(new ExtractedRepresentation
                    {
                        ExternalId = user.Id,
                        Version = user.Version
                    });
                }
            }

            return result;
        }
    }
}

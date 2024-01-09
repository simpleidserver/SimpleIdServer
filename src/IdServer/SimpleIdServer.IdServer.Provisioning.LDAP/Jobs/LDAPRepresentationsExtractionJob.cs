// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Jobs;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;

namespace SimpleIdServer.IdServer.Provisioning.LDAP.Jobs
{
    public class LDAPRepresentationsExtractionJob : RepresentationExtractionJob<LDAPRepresentationsExtractionJobOptions>
    {
        private readonly IProvisioningService _provisioningService;
        public const string NAME = "LDAP";

        public LDAPRepresentationsExtractionJob(IEnumerable<IProvisioningService> provisioningServices, Microsoft.Extensions.Configuration.IConfiguration configuration, ILogger<RepresentationExtractionJob<LDAPRepresentationsExtractionJobOptions>> logger, IBusControl busControl, IIdentityProvisioningStore identityProvisioningStore, IExtractedRepresentationRepository extractedRepresentationRepository, IOptions<IdServerHostOptions> options) : base(configuration, logger, busControl, identityProvisioningStore, extractedRepresentationRepository, options)
        {
            _provisioningService = provisioningServices.Single(p => p.Name == NAME);
        }

        public override string Name => NAME;

        protected override async IAsyncEnumerable<List<Domains.ExtractedRepresentation>> FetchUsers(LDAPRepresentationsExtractionJobOptions options, string destinationFolder, IdentityProvisioning identityProvisioning)
        {
            await foreach (var extractedResult in _provisioningService.Extract(options, identityProvisioning.Definition))
            {
                var result = WriteFile(extractedResult, destinationFolder, identityProvisioning.Definition);
                yield return result;
            }
        }

        private List<Domains.ExtractedRepresentation> WriteFile(ExtractedResult extractedResult, string destinationFolder, IdentityProvisioningDefinition definition)
        {
            var result = new List<Domains.ExtractedRepresentation>();
            using (var fs = File.CreateText(Path.Combine(destinationFolder, $"{extractedResult.CurrentPage}.csv")))
            {
                fs.WriteLine(BuildFileColumns(definition));
                foreach (var user in extractedResult.Users)
                {
                    fs.WriteLine($"{user.Id}{Constants.IdProviderSeparator}{user.Version}{Constants.IdProviderSeparator}{string.Join(Constants.IdProviderSeparator, user.Values)}");
                    result.Add(new Domains.ExtractedRepresentation
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
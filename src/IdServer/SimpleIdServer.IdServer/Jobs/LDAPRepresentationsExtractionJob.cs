// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.IO;
using System.Linq;
using System.Net;

namespace SimpleIdServer.IdServer.Jobs
{
    public class LDAPRepresentationsExtractionJob : RepresentationExtractionJob<LDAPRepresentationsExtractionJobOptions>
    {
        public const string NAME = "LDAP";

        public LDAPRepresentationsExtractionJob(ILogger<RepresentationExtractionJob<LDAPRepresentationsExtractionJobOptions>> logger, IBusControl busControl, IIdentityProvisioningStore identityProvisioningStore, IExtractedRepresentationRepository extractedRepresentationRepository, IOptions<IdServerHostOptions> options) : base(logger, busControl, identityProvisioningStore, extractedRepresentationRepository, options)
        {
        }

        public override string Name => NAME;

        protected override async IAsyncEnumerable<List<ExtractedRepresentation>> FetchUsers(LDAPRepresentationsExtractionJobOptions options, string destinationFolder, IdentityProvisioning identityProvisioning)
        {
            var pr = new PageResultRequestControl(options.BatchSize);
            var request = new SearchRequest(options.UsersDN, $"(&{string.Join(string.Empty, options.UserObjectClasses.Split(',').Select(o => $"(objectClass={o})"))})", SearchScope.Subtree);
            request.Controls.Add(pr);
            var credentials = new NetworkCredential(options.BindDN, options.BindCredentials);
            int currentPage = 0;
            using (var connection = new LdapConnection(new LdapDirectoryIdentifier(options.Server, options.Port), credentials, AuthType.Basic))
            {
                connection.SessionOptions.ProtocolVersion = 3;
                connection.Bind();
                while(true)
                {
                    var response = (SearchResponse)connection.SendRequest(request);
                    if (!response.Controls.Any()) break;
                    var pageResponse = (PageResultResponseControl)response.Controls[0];
                    var extractedRepresentations = ExtractUsers(response.Entries, currentPage, destinationFolder, options, identityProvisioning.Definition);
                    pr.Cookie = pageResponse.Cookie;
                    yield return extractedRepresentations;
                    if (!pageResponse.Cookie.Any()) break;
                    currentPage++;
                }
            }
        }

        private List<ExtractedRepresentation> ExtractUsers(SearchResultEntryCollection entries, int currentPage, string destinationFolder, LDAPRepresentationsExtractionJobOptions options, IdentityProvisioningDefinition definition)
        {
            var result = new List<ExtractedRepresentation>();
            using (var fs = File.CreateText(Path.Combine(destinationFolder, $"{currentPage}.csv")))
            {
                fs.WriteLine(BuildFileColumns(definition));
                foreach (SearchResultEntry entry in entries)
                {
                    var userId = GetUserId(entry, options);
                    var version = GetVersion(entry, options);
                    result.Add(new ExtractedRepresentation
                    {
                        ExternalId = userId,
                        Version = version
                    });
                    fs.WriteLine($"{userId}{Constants.IdProviderSeparator}{version}{Constants.IdProviderSeparator}{Extract(entry, definition)}");
                }
            }

            return result;
        }

        private string Extract(SearchResultEntry result, IdentityProvisioningDefinition definition)
        {
            var lst = new List<string>();
            foreach(var mappingRule in definition.MappingRules)
            {
                if (!result.Attributes.Contains(mappingRule.From))
                {
                    if (mappingRule.From == "distinguishedName") lst.Add(result.DistinguishedName);
                    continue;
                }

                var record = result.Attributes[mappingRule.From][0];
                lst.Add(record.ToString());
            }

            return string.Join(Constants.IdProviderSeparator, lst);
        }

        private string GetUserId(SearchResultEntry entry, LDAPRepresentationsExtractionJobOptions options)
        {
            if (!entry.Attributes.Contains(options.UUIDLDAPAttribute)) return entry.DistinguishedName;
            return entry.Attributes[options.UUIDLDAPAttribute][0].ToString();
        }

        private string GetVersion(SearchResultEntry entry, LDAPRepresentationsExtractionJobOptions options)
        {
            if (!entry.Attributes.Contains(options.ModificationDateAttribute)) return Guid.NewGuid().ToString();
            return entry.Attributes[options.ModificationDateAttribute][0].ToString();
        }
    }
}

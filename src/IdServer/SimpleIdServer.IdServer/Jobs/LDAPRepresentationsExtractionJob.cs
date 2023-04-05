// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
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

        public LDAPRepresentationsExtractionJob(ILogger<RepresentationExtractionJob<LDAPRepresentationsExtractionJobOptions>> logger, IIdentityProvisioningStore identityProvisioningStore, IExtractedRepresentationRepository extractedRepresentationRepository, IOptions<IdServerHostOptions> options) : base(logger, identityProvisioningStore, extractedRepresentationRepository, options)
        {
        }

        public override string Name => NAME;

        /*
        const string userName = "cn=admin,dc=xl,dc=com";
        const string password = "password";
        const string userStore = "ou=people,dc=xl,dc=com";
        var credentials = new NetworkCredential(userName, password);
        var pr = new PageResultRequestControl(1);
        var request = new SearchRequest(userStore, "(&(objectClass=organizationalPerson)(objectClass=person))", System.DirectoryServices.Protocols.SearchScope.Subtree);
        request.Controls.Add(pr);
        using (var connection = new LdapConnection(new LdapDirectoryIdentifier("localhost", 389), credentials, AuthType.Basic))
        {
            connection.SessionOptions.ProtocolVersion = 3;
            connection.Bind();
            while(true)
            {
                var response = (SearchResponse)connection.SendRequest(request);
                if (!response.Controls.Any()) break;
                var pageResponse = (PageResultResponseControl)response.Controls[0];
                if (!pageResponse.Cookie.Any()) break;
                // Extract all the users
                pr.Cookie = pageResponse.Cookie;
            }
        }
        */

        protected override async IAsyncEnumerable<List<ExtractedRepresentation>> FetchUsers(LDAPRepresentationsExtractionJobOptions options, string destinationFolder, IdentityProvisioning identityProvisioning)
        {
            var pr = new PageResultRequestControl(options.BatchSize);
            var request = new SearchRequest(options.UsersDN, $"(&{string.Join(string.Empty, options.UserObjectClasses.Split(',').Select(o => $"(objectClass={o})"))})", SearchScope.Subtree);
            request.Controls.Add(pr);
            var credentials = new NetworkCredential(options.BindDN, options.BindCredentials);
            using (var connection = new LdapConnection(new LdapDirectoryIdentifier(options.Server, options.Port), credentials, AuthType.Basic))
            {
                connection.SessionOptions.ProtocolVersion = 3;
                connection.Bind();
                while(true)
                {
                    var response = (SearchResponse)connection.SendRequest(request);
                    if (!response.Controls.Any()) break;
                    var pageResponse = (PageResultResponseControl)response.Controls[0];
                    if (!pageResponse.Cookie.Any()) break;
                    // Extract all the users
                    pr.Cookie = pageResponse.Cookie;
                    yield return new List<ExtractedRepresentation>();
                }
            }
        }

        private void ExtractUsers(SearchResultEntryCollection entries, int currentPage, string destinationFolder, LDAPRepresentationsExtractionJobOptions options, IdentityProvisioningDefinition definition)
        {
            using (var fs = File.CreateText(Path.Combine(destinationFolder, $"{currentPage}.csv")))
            {
                fs.WriteLine(BuildFileColumns(definition));
                foreach (SearchResultEntry entry in entries)
                    fs.WriteLine($"{GetUserId(entry, options)}{Constants.SEPARATOR}{resource.Meta.Version}{Constants.SEPARATOR}{Extract(resource, definition)}");
            }
        }

        private string Extract(SearchResultEntry result, IdentityProvisioningDefinition definition)
        {
            return string.Empty;
        }

        private string GetUserId(SearchResultEntry entry, LDAPRepresentationsExtractionJobOptions options) => entry.Attributes[options.UUIDLDAPAttribute][0].ToString();
    }
}

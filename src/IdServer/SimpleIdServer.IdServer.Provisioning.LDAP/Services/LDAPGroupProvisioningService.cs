// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MassTransit;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Jobs;
using SimpleIdServer.IdServer.Provisioning.LDAP.Jobs;
using System.DirectoryServices.Protocols;
using System.Net;

namespace SimpleIdServer.IdServer.Provisioning.LDAP.Services;

public class LDAPGroupProvisioningService : IGroupsProvisioningService
{
    public string Name => LDAPRepresentationsExtractionJob.NAME;

    public async Task Extract(object obj, IdentityProvisioningDefinition definition)
    {
        var options = obj as LDAPRepresentationsExtractionJobOptions;
        var pr = new PageResultRequestControl(options.BatchSize);
        var request = new SearchRequest(options.GroupsDN, $"(&{string.Join(string.Empty, options.GroupObjectClasses.Split(',').Select(o => $"(objectClass={o})"))})", SearchScope.Subtree);
        request.Controls.Add(pr);
        var credentials = new NetworkCredential(options.BindDN, options.BindCredentials);
        int currentPage = 0;
        using (var connection = new LdapConnection(new LdapDirectoryIdentifier(options.Server, options.Port), credentials, AuthType.Basic))
        {
            connection.SessionOptions.ProtocolVersion = 3;
            connection.Bind();
            while (true)
            {
                var response = (SearchResponse)connection.SendRequest(request);
                if (!response.Controls.Any()) break;
                var pageResponse = (PageResultResponseControl)response.Controls[0];
                /*
                var extractionResultLst = ExtractUsers(response.Entries, options, definition);
                */
                var record = new ExtractedResult { CurrentPage = currentPage, Users = extractionResultLst };
                pr.Cookie = pageResponse.Cookie;
                yield return record;
                if (!pageResponse.Cookie.Any()) break;
                currentPage++;
            }
        }
    }

    private List<ExtractedGroupResult> ExtractGroups(SearchResultEntryCollection entries, LDAPRepresentationsExtractionJobOptions options, IdentityProvisioningDefinition definition)
    {
        var result = new List<ExtractedGroupResult>();
        foreach(SearchResultEntry entry in entries)
        {
            result.Add(new ExtractedGroupResult
            {

            });
        }

        return result;
    }

    private List<string> ExtractGroup()
    {
        return null;
    }
}

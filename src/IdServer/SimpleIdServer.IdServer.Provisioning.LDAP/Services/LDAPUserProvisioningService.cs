// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Jobs;
using SimpleIdServer.IdServer.Provisioning.LDAP.Jobs;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Provisioning.LDAP.Services;

public class LDAPUserProvisioningService : IUserProvisioningService
{
    public string Name => LDAPRepresentationsExtractionJob.NAME;

    public async IAsyncEnumerable<ExtractedResult> Extract(object obj, IdentityProvisioningDefinition definition)
    {
        var options = obj as LDAPRepresentationsExtractionJobOptions;
        var pr = new PageResultRequestControl(options.BatchSize);
        var request = new SearchRequest(options.UsersDN, $"(&{string.Join(string.Empty, options.UserObjectClasses.Split(',').Select(o => $"(objectClass={o})"))})", SearchScope.Subtree);
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
                var record = Extract(response.Entries, options, definition, currentPage, connection);
                pr.Cookie = pageResponse.Cookie;
                yield return record;
                if (!pageResponse.Cookie.Any()) break;
                currentPage++;
            }
        }
    }

    public async Task<ExtractedResult> ExtractTestData(object obj, IdentityProvisioningDefinition definition)
    {
        var options = obj as LDAPRepresentationsExtractionJobOptions;
        var pr = new PageResultRequestControl(options.BatchSize);
        var request = new SearchRequest(options.UsersDN, $"(&{string.Join(string.Empty, options.UserObjectClasses.Split(',').Select(o => $"(objectClass={o})"))})", SearchScope.Subtree);
        request.Controls.Add(pr);
        var credentials = new NetworkCredential(options.BindDN, options.BindCredentials);
        int currentPage = 0;
        using (var connection = new LdapConnection(new LdapDirectoryIdentifier(options.Server, options.Port), credentials, AuthType.Basic))
        {
            connection.SessionOptions.ProtocolVersion = 3;
            connection.Bind();
            var response = (SearchResponse)connection.SendRequest(request);
            if (!response.Controls.Any()) return new ExtractedResult();
            var pageResponse = (PageResultResponseControl)response.Controls[0];
            var record = Extract(response.Entries, options, definition, currentPage, connection);
            return record;
        }
    }

    public Task<IEnumerable<string>> GetAllowedAttributes(object obj)
    {
        var result = new List<string>();
        var options = obj as LDAPRepresentationsExtractionJobOptions;
        var userObjectClassLst = options.UserObjectClasses.Split(',');
        var pr = new PageResultRequestControl(1);
        var credentials = new NetworkCredential(options.BindDN, options.BindCredentials);
        using (var connection = new LdapConnection(new LdapDirectoryIdentifier(options.Server, options.Port), credentials, AuthType.Basic))
        {
            connection.SessionOptions.ProtocolVersion = 3;
            connection.Bind();
            foreach (var userObjectClass in userObjectClassLst)
            {
                var request = new SearchRequest(options.UsersDN, $"(objectClass={userObjectClass})", System.DirectoryServices.Protocols.SearchScope.Subtree);
                request.Controls.Add(pr);
                var response = (SearchResponse)connection.SendRequest(request);
                var entries = response.Entries;
                if (entries.Count == 0) continue;
                var firstEntry = entries[0] as SearchResultEntry;
                foreach(var attr in firstEntry.Attributes.AttributeNames)
                {
                    if (result.Contains(attr.ToString())) continue;
                    result.Add(attr.ToString());
                }
            }
        }

        return Task.FromResult((IEnumerable<string>)result.Distinct().OrderBy(r => r).ToList());
    }

    private ExtractedResult Extract(SearchResultEntryCollection entries, LDAPRepresentationsExtractionJobOptions options, IdentityProvisioningDefinition definition, int currentPage, LdapConnection ldapConnection)
    {
        var users = new List<ExtractedUser>();
        var groups = new List<ExtractedGroup>();
        foreach(SearchResultEntry entry in entries)
        {
            var userId = GetRepresentationId(entry, options);
            var version = GetVersion(entry, options);
            users.Add(new ExtractedUser
            {
                Id = userId,
                Values = ExtractUser(entry, definition),
                Version = version
            });
            groups.AddRange(ResolveUserGroups(userId, entry, options, ldapConnection, definition));
        }

        return new ExtractedResult { Users = users, CurrentPage = currentPage, Groups = groups };
    }

    private List<ExtractedGroup> ResolveUserGroups(string userId, SearchResultEntry userEntry, LDAPRepresentationsExtractionJobOptions options, LdapConnection connection, IdentityProvisioningDefinition definition)
    {
        var result = new List<ExtractedGroup>();
        if(options.RetrievingStrategies == LoadingStrategies.LOAD_BY_MEMBER_ATTRIBUTE)
        {
            if (!userEntry.Attributes.Contains(options.MembershipUserLDAPAttribute)) return result;
            var userGroupId = userEntry.Attributes[options.MembershipUserLDAPAttribute][0].ToString();
            var groupObjectClassLst = options.GroupObjectClasses.Split(',');
            foreach(var groupObjectClass in groupObjectClassLst)
            {
                var request = new SearchRequest(options.UsersDN, $"&((objectClass={groupObjectClass})({options.MembershipLDAPAttribute}={userGroupId}))", System.DirectoryServices.Protocols.SearchScope.Subtree);
                var response = (SearchResponse)connection.SendRequest(request);
                var entries = response.Entries;
                if (entries.Count == 0) continue;
                result.AddRange(ExtractGroups(userId, entries, options, definition));
            }

            return result;
        }

        if(options.RetrievingStrategies == LoadingStrategies.LOAD_FROM_USER_MEMBEROF_ATTRIBUTE)
        {
            foreach(DirectoryAttribute attr in userEntry.Attributes)
            {
                if (attr.Name != options.MemberOfAttribute) continue;
                var values = (string[])attr.GetValues(typeof(string));
                if (values == null || values.Count() > 1) continue;
                var dn = values.First();
                var request = new SearchRequest(options.UsersDN, $"&((distinguishedName={dn}))", System.DirectoryServices.Protocols.SearchScope.Subtree);
                var response = (SearchResponse)connection.SendRequest(request);
                var entries = response.Entries;
                if (entries.Count == 0) continue;
                result.AddRange(ExtractGroups(userId, entries, options, definition));
            }
        }

        return result;
    }

    private List<ExtractedGroup> ExtractGroups(string userId, SearchResultEntryCollection entries, LDAPRepresentationsExtractionJobOptions options, IdentityProvisioningDefinition definition)
    {
        var result = new List<ExtractedGroup>();
        foreach (SearchResultEntry entry in entries)
        {
            var groupId = GetRepresentationId(entry, options);
            var version = GetVersion(entry, options);
            result.Add(new ExtractedGroup
            {
                Id = groupId,
                Values = ExtractUser(entry, definition),
                Version = version,
                UserId = userId
            });
        }

        return result;
    }

    private List<string> ExtractUser(SearchResultEntry entry, IdentityProvisioningDefinition definition)
    {
        var invalidMappingRules = new List<string>();
        var lst = new List<string>();
        foreach (var mappingRule in definition.MappingRules)
        {
            if (!entry.Attributes.Contains(mappingRule.From))
            {
                if (mappingRule.From == "distinguishedName") lst.Add(entry.DistinguishedName);
                continue;
            }

            var attribute = entry.Attributes[mappingRule.From];
            if (mappingRule.MapperType == MappingRuleTypes.USERATTRIBUTE && !mappingRule.HasMultipleAttribute && attribute.Count > 1)
            {
                invalidMappingRules.Add($"mapping rule '{mappingRule.From}' is not configured to fetch more than one attribute");
                continue;
            }

            if (attribute.Count == 1)
            {
                var record = entry.Attributes[mappingRule.From][0];
                lst.Add(record.ToString());
            }
            else
            {
                var records = new List<string>();
                foreach (var record in entry.Attributes[mappingRule.From])
                    records.Add(Encoding.UTF8.GetString((byte[])record));

                lst.Add(JsonSerializer.Serialize(records));
            }
        }

        if (invalidMappingRules.Any()) throw new InvalidOperationException(string.Join(",", invalidMappingRules.Distinct()));
        return lst;
    }

    private string GetRepresentationId(SearchResultEntry entry, LDAPRepresentationsExtractionJobOptions options)
    {
        if (!entry.Attributes.Contains(options.UUIDLDAPAttribute)) return entry.DistinguishedName;
        var result = entry.Attributes[options.UUIDLDAPAttribute][0];
        var payload = result as byte[];
        if(payload != null)
        {
            return (new SecurityIdentifier(payload, 0)).ToString();
        }

        return result.ToString();
    }

    private string GetVersion(SearchResultEntry entry, LDAPRepresentationsExtractionJobOptions options)
    {
        if (!entry.Attributes.Contains(options.ModificationDateAttribute)) return Guid.NewGuid().ToString();
        return entry.Attributes[options.ModificationDateAttribute][0].ToString();
    }
}
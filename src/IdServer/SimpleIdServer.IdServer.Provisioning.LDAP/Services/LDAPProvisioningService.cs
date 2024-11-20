// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Configuration;
using SimpleIdServer.IdServer.Domains;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Provisioning.LDAP.Services;

public class LDAPProvisioningService : BaseProvisioningService<LDAPRepresentationsExtractionJobOptions>
{
    public LDAPProvisioningService(IConfiguration configuration) : base(configuration)
    {

    }

    public override string Name => NAME;

    public static string NAME = "LDAP";

    public override Task<ExtractedResult> ExtractTestData(IdentityProvisioningDefinition definition, CancellationToken cancellationToken)
    {
        return Extract(new ExtractionPage
        {
            Page = 1
        }, definition, cancellationToken);
    }

    public override Task<ExtractedResult> Extract(ExtractionPage currentPage, IdentityProvisioningDefinition definition, CancellationToken cancellationToken)
    {
        int page = 1;
        var options = GetOptions(definition);
        var pr = new PageResultRequestControl(options.BatchSize);
        var request = new SearchRequest(options.UsersDN, $"(&{string.Join(string.Empty, options.UserObjectClasses.Split(',').Select(o => $"(objectClass={o})"))})", SearchScope.Subtree);
        request.Controls.Add(pr);
        using (var connection = BuildConnection(options))
        {
            while(true)
            {
                var response = (SearchResponse)connection.SendRequest(request);
                if(page == currentPage.Page)
                {
                    var record = Extract(response.Entries, options, definition, connection);
                    return Task.FromResult(record);
                }

                var pageResponse = (PageResultResponseControl)response.Controls[0];
                pr.Cookie = pageResponse.Cookie;
                if (!pageResponse.Cookie.Any()) break;
                page++;
            }
        }

        return Task.FromResult((ExtractedResult)null);
    }

    public override async Task<List<ExtractionPage>> Paginate(IdentityProvisioningDefinition definition, CancellationToken cancellationToken)
    {
        var result = new List<ExtractionPage>();
        int currentPage = 1;
        var options = GetOptions(definition);
        var pr = new PageResultRequestControl(options.BatchSize);
        var request = new SearchRequest(options.UsersDN, $"(&{string.Join(string.Empty, options.UserObjectClasses.Split(',').Select(o => $"(objectClass={o})"))})", SearchScope.Subtree);
        request.Controls.Add(pr);
        using (var connection = BuildConnection(options))
        {
            while(true)
            {
                var response = (SearchResponse)connection.SendRequest(request);
                if (!response.Controls.Any()) break;
                var pageResponse = (PageResultResponseControl)response.Controls[0];
                result.Add(new ExtractionPage
                {
                    BatchSize = options.BatchSize,
                    Page = currentPage
                });
                pr.Cookie = pageResponse.Cookie;
                if (!pageResponse.Cookie.Any()) break;
                currentPage++;
            }
        }

        return result;
    }

    private ExtractedResult Extract(SearchResultEntryCollection entries, LDAPRepresentationsExtractionJobOptions options, IdentityProvisioningDefinition definition, LdapConnection ldapConnection)
    {
        var users = new List<ExtractedUser>();
        var groups = new List<ExtractedGroup>();
        foreach(SearchResultEntry entry in entries)
        {
            var userId = GetRepresentationId(entry, options, true);
            var version = GetVersion(entry, options);
            var user = new ExtractedUser
            {
                Id = userId,
                Values = ExtractRepresentation(entry, definition, IdentityProvisioningMappingUsage.USER),
                Version = version
            };
            users.Add(user);
            if(options.IsGroupSyncEnabled)
            {
                var userGroups = ResolveUserGroups(userId, entry, options, ldapConnection, definition);
                groups.AddRange(userGroups);
                user.GroupIds = userGroups.Select(g => g.Id).ToList();
            }
        }

        return new ExtractedResult { Users = users, Groups = groups };
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
                var request = new SearchRequest(options.GroupsDN, $"(&(objectClass={groupObjectClass})({options.MembershipLDAPAttribute}={userGroupId}))", SearchScope.OneLevel);
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
                var splitted = dn.Split(',');
                var rootDN = splitted.Skip(1).Join(",");
                var request = new SearchRequest(rootDN, $"({splitted.First()})", System.DirectoryServices.Protocols.SearchScope.OneLevel);
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
            var groupId = GetRepresentationId(entry, options, false);
            var version = GetVersion(entry, options);
            result.Add(new ExtractedGroup
            {
                Id = groupId,
                Values = ExtractRepresentation(entry, definition, IdentityProvisioningMappingUsage.GROUP),
                Version = version
            });
        }

        return result;
    }

    private List<string> ExtractRepresentation(SearchResultEntry entry, IdentityProvisioningDefinition definition, IdentityProvisioningMappingUsage usage)
    {
        var invalidMappingRules = new List<string>();
        var lst = new List<string>();
        foreach (var mappingRule in definition.MappingRules.Where(r => r.Usage == usage))
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

    private string GetRepresentationId(SearchResultEntry entry, LDAPRepresentationsExtractionJobOptions options, bool isUser = true)
    {
        var attributeName = isUser ? options.UserIdLDAPAttribute : options.GroupIdLDAPAttribute;
        if (!entry.Attributes.Contains(attributeName)) return entry.DistinguishedName;
        var result = entry.Attributes[attributeName][0];
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
    
    private LdapConnection BuildConnection(LDAPRepresentationsExtractionJobOptions options)
    {
        var credentials = new NetworkCredential(options.BindDN, options.BindCredentials);
        var connection = new LdapConnection(new LdapDirectoryIdentifier(options.Server, options.Port), credentials, AuthType.Basic);
        connection.SessionOptions.ProtocolVersion = 3;
        connection.Bind();
        return connection;
    }
}
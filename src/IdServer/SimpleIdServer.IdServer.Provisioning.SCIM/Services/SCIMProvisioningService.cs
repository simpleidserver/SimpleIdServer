// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using BlushingPenguin.JsonPath;
using Microsoft.Extensions.Configuration;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.Scim.Client;
using SimpleIdServer.Scim.Client.DTOs;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Provisioning.SCIM.Services;

public class SCIMProvisioningService : BaseProvisioningService<SCIMRepresentationsExtractionJobOptions>
{
    public SCIMProvisioningService(IConfiguration configuration) : base(configuration)
    {
    }

    public override string Name => NAME;

    public static string NAME = "SCIM";

    public override Task<ExtractedResult> ExtractTestData(IdentityProvisioningDefinition definition, CancellationToken cancellationToken)
    {
        var options = GetOptions(definition);
        return Extract(new ExtractionPage { BatchSize = options.Count, Page = 1 }, definition, cancellationToken);
    }

    public override async Task<ExtractedResult> Extract(ExtractionPage currentPage, IdentityProvisioningDefinition definition, CancellationToken cancellationToken)
    {
        var options = GetOptions(definition);
        var page = currentPage.Page;
        using (var scimClient = new SCIMClient(options.SCIMEdp))
        {
            var accessToken = await GetAccessToken(options);
            var searchUsers = await scimClient.SearchUsers(new Scim.Client.SearchRequest
            {
                Count = currentPage.BatchSize,
                StartIndex = page == 1 ? 1 : page * currentPage.BatchSize
            }, accessToken, cancellationToken);
            var result = await Extract(searchUsers.Item1.Resources, definition, scimClient, options, CancellationToken.None);
            return result;
        }
    }

    public override async Task<List<ExtractionPage>> Paginate(IdentityProvisioningDefinition definition, CancellationToken cancellationToken)
    {
        var options = GetOptions(definition);
        using (var scimClient = new SCIMClient(options.SCIMEdp))
        {
            var accessToken = await GetAccessToken(options);
            var searchUsers = await scimClient.SearchUsers(new Scim.Client.SearchRequest
            {
                Count = options.Count,
                StartIndex = 1
            }, accessToken, cancellationToken);
            var totalResults = searchUsers.Item1.TotalResults;
            var totalPage = ((int)Math.Ceiling((double)totalResults / options.Count));
            var result = Enumerable.Range(1, totalPage);
            return result.Select(p => new ExtractionPage
            {
                BatchSize = options.Count,
                Page = p
            }).ToList();
        }
    }

    private async Task<ExtractedResult> Extract(IEnumerable<RepresentationResult> resources, IdentityProvisioningDefinition definition, SCIMClient client, SCIMRepresentationsExtractionJobOptions options, CancellationToken cancellationToken)
    {
        var result = new ExtractedResult();
        foreach (var resource in resources)
        {
            var user = ExtractUser(resource, definition);
            result.Users.Add(user);
            var jsonDoc = JsonDocument.Parse(resource.AdditionalData.ToJsonString());
            var memberIdsElt = jsonDoc.SelectTokens("$.groups[*].value");
            if (memberIdsElt == null || !memberIdsElt.Any()) continue;
            var groupIds = memberIdsElt.Select(e => e.GetString()).ToList();
            user.GroupIds = groupIds;
            result.Groups.AddRange(await ExtractGroups(groupIds, client, options, definition, cancellationToken));
        }

        return result;
    }

    private async Task<List<ExtractedGroup>> ExtractGroups(List<string> groupIds, SCIMClient client, SCIMRepresentationsExtractionJobOptions options, IdentityProvisioningDefinition definition, CancellationToken cancellationToken)
    {
        var result = new List<ExtractedGroup>();
        var accessToken = await GetAccessToken(options);
        foreach(var groupId in groupIds)
        {
            var group = await client.GetGroup(groupId, accessToken, cancellationToken);
            result.Add(ExtractGroup(group, definition));
        }

        return result;
    }

    private ExtractedUser ExtractUser(RepresentationResult resource, IdentityProvisioningDefinition definition)
    {
        var values = ExtractRepresentation(resource, definition, IdentityProvisioningMappingUsage.USER);
        return new ExtractedUser
        {
            Id = resource.Id,
            Version = resource.Meta.Version.ToString(),
            Values = values
        };
    }

    private ExtractedGroup ExtractGroup(RepresentationResult resource, IdentityProvisioningDefinition definition)
    {
        var values = ExtractRepresentation(resource, definition, IdentityProvisioningMappingUsage.GROUP);
        return new ExtractedGroup
        {
            Id = resource.Id,
            Version = resource.Meta.Version.ToString(),
            Values = values
        };
    }

    private List<string> ExtractRepresentation(RepresentationResult resource, IdentityProvisioningDefinition definition, IdentityProvisioningMappingUsage usage)
    {
        var jsonDoc = JsonDocument.Parse(resource.AdditionalData.ToJsonString());
        var values = new List<string>();
        var invalidMappingRules = new List<string>();
        foreach (var mappingRule in definition.MappingRules.Where(r => r.Usage == usage))
        {
            var tokens = jsonDoc.SelectTokens(mappingRule.From);
            if (tokens.Count() == 0)
            {
                values.Add(string.Empty);
                continue;
            }

            var firstToken = tokens.First();
            if (firstToken.ValueKind == JsonValueKind.Object)
            {
                invalidMappingRules.Add($"mapping rule '{mappingRule.From}' tried to fetch a complex element");
                continue;
            }

            var lstValues = tokens.Select(t => t.GetString());
            if (!mappingRule.HasMultipleAttribute && lstValues.Count() > 1 && mappingRule.MapperType == MappingRuleTypes.USERATTRIBUTE)
            {
                invalidMappingRules.Add($"mapping rule '{mappingRule.From}' is not configured to fetch more than one attribute");
                continue;
            }

            if (lstValues.Count() == 1) values.Add(lstValues.First());
            else
            {
                var str = JsonSerializer.Serialize(lstValues);
                values.Add(str);
            }
        }

        if (invalidMappingRules.Any()) throw new InvalidOperationException(string.Join(",", invalidMappingRules.Distinct()));
        return values;
    }

    private Task<string> GetAccessToken(SCIMRepresentationsExtractionJobOptions options)
    {
        switch (options.AuthenticationType)
        {
            case ClientAuthenticationTypes.APIKEY:
                return Task.FromResult(options.ApiKey);
            default:
                throw new NotImplementedException($"Authentication {options.AuthenticationType} is not supported");
        }
    }
}

// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using BlushingPenguin.JsonPath;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Jobs;
using SimpleIdServer.IdServer.Provisioning.SCIM.Jobs;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.Scim.Client;
using SimpleIdServer.Scim.Client.DTOs;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Provisioning.SCIM.Services;

public class SCIMProvisioningService : IProvisioningService
{
    private readonly IExtractedRepresentationRepository _extractedRepresentationRepository;

    public SCIMProvisioningService(IExtractedRepresentationRepository extractedRepresentationRepository)
    {
        _extractedRepresentationRepository = extractedRepresentationRepository;
    }

    public string Name => SCIMRepresentationsExtractionJob.NAME;

    public async IAsyncEnumerable<ExtractedResult> Extract(object obj, IdentityProvisioningDefinition definition)
    {
        var options = obj as SCIMRepresentationsExtractionJobOptions;
        using (var scimClient = new SCIMClient(options.SCIMEdp))
        {
            var accessToken = await GetAccessToken(options);
            var searchUsers = await scimClient.SearchUsers(new Scim.Client.SearchRequest
            {
                Count = options.Count,
                StartIndex = 1
            }, accessToken, CancellationToken.None);
            var filterUsers = await FilterUsers(searchUsers.Item1);
            var result = ExtractUsers(filterUsers, 0, definition);
            yield return result;
            var totalResults = searchUsers.Item1.TotalResults;
            var count = searchUsers.Item1.ItemsPerPage;
            var nbPages = ((int)Math.Ceiling((double)totalResults / count)) - 1;
            var allPages = Enumerable.Range(1, nbPages);
            foreach (var currentPage in allPages)
            {
                var newSearchUsers = await scimClient.SearchUsers(new Scim.Client.SearchRequest
                {
                    Count = count,
                    StartIndex = currentPage * count
                }, accessToken, CancellationToken.None);
                var newFilterUsers = FilterUsers(newSearchUsers.Item1).Result;
                result = ExtractUsers(newFilterUsers, 1, definition);
                yield return result;
            }
        }
    }

    public async Task<ExtractedResult> ExtractTestData(object obj, IdentityProvisioningDefinition definition)
    {
        var options = obj as SCIMRepresentationsExtractionJobOptions;
        using (var scimClient = new SCIMClient(options.SCIMEdp))
        {
            var accessToken = await GetAccessToken(options);
            var searchUsers = await scimClient.SearchUsers(new Scim.Client.SearchRequest
            {
                Count = options.Count,
                StartIndex = 1
            }, accessToken, CancellationToken.None);
            var result = ExtractUsers(searchUsers.Item1.Resources, 1, definition);
            return result;
        }
    }

    public async Task<IEnumerable<string>> GetAllowedAttributes(object obj)
    {
        const string userResourceId = "urn:ietf:params:scim:schemas:core:2.0:User";
        var options = obj as SCIMRepresentationsExtractionJobOptions;
        using (var scimClient = new SCIMClient(options.SCIMEdp))
        {
            var schema = await scimClient.GetSchemas(CancellationToken.None);
            var resource = schema.Resources.Single(r => r.Id == userResourceId);
            return GetAllowedAttributes(resource);
        }
    }

    private ExtractedResult ExtractUsers(IEnumerable<RepresentationResult> resources, int currentPage, IdentityProvisioningDefinition definition)
    {
        var result = new ExtractedResult();
        foreach (var resource in resources) result.Users.Add(ExtractUser(resource, definition));
        result.CurrentPage = currentPage;
        return result;
    }

    private ExtractedUserResult ExtractUser(RepresentationResult resource, IdentityProvisioningDefinition definition)
    {
        var jsonDoc = JsonDocument.Parse(resource.AdditionalData.ToJsonString());
        var values = new List<string>();
        var invalidMappingRules = new List<string>();
        foreach (var mappingRule in definition.MappingRules)
        {
            var tokens = jsonDoc.SelectTokens(mappingRule.From);
            if (tokens.Count() == 0)
            {
                values.Add(string.Empty);
                continue;
            }

            var firstToken = tokens.First();
            if(firstToken.ValueKind == JsonValueKind.Object)
            {
                invalidMappingRules.Add($"mapping rule '{mappingRule.From}' tried to fetch a complex element");
                continue;
            }

            var lstValues = tokens.Select(t => t.GetString());
            if(!mappingRule.HasMultipleAttribute && lstValues.Count() > 1 && mappingRule.MapperType == MappingRuleTypes.USERATTRIBUTE)
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
        return new ExtractedUserResult
        {
            Id = resource.Id,
            Version = resource.Meta.Version.ToString(),
            Values = values
        };
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

    private List<string> GetAllowedAttributes(SchemaResourceResult schema)
    {
        var result = new List<string>();
        foreach(var attr in schema.Attributes)
        {
            var path = $"$.{attr.Name}";
            var isComplexMultiValued = attr.Type == "complex" && attr.MultiValued;
            if (!isComplexMultiValued) result.Add(path);
            if (attr.SubAttributes == null) continue;

            foreach (var subAttr in attr.SubAttributes)
                result.Add(isComplexMultiValued ? $"{path}[*].{subAttr.Name}" : $"{path}.{subAttr.Name}");
        }

        return result.Distinct().OrderBy(s => s).ToList();
    }
}

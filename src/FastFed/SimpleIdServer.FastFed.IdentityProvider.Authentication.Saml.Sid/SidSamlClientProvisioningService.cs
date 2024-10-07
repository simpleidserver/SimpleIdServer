// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Options;
using SimpleIdServer.FastFed.Authentication.Saml;
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.IdentityProvider.Authentication.Saml.Sid;

public class SidSamlClientProvisioningService : ISamlClientProvisioningService
{
    private Dictionary<string, string> scimToSamlAttributes = new Dictionary<string, string>
    {
        { "externalId", "externalId" },
        { "userName", "userName" },
        { "displayName", "displayName"  },
        { "name.givenName", "givenName" },
        { "name.familyName", "familyName" },
        { "name.middleName", "middleName" },
        { "emails[primary eq true].value", "email" },
        { "phoneNumbers[primary eq true].value", "phoneNumber" }
    };
    private readonly IdServer.Helpers.IHttpClientFactory _httpClientFactory;
    private readonly FastFedSidSamlAuthenticationOptions _options;

    public SidSamlClientProvisioningService(
        IdServer.Helpers.IHttpClientFactory httpClientFactory,
        IOptions<FastFedSidSamlAuthenticationOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    public async Task Provision(string clientId, string metadataUrl, SamlEntrepriseMappingsResult mappings, CancellationToken cancellationToken)
    {
        using (var httpClient = _httpClientFactory.GetHttpClient())
        {
            var accessToken = await GetAccessToken(httpClient);
            var newScope = await CreateScope(httpClient, clientId, mappings, accessToken);
            var client = new IdServer.Domains.Client
            {
                Id = Guid.NewGuid().ToString(),
                ClientSecret = Guid.NewGuid().ToString(),
                ClientId = clientId,
                ClientType = "SAML",
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                Parameters = new JsonObject
                {
                    { "SAML2_SP_METADATA", metadataUrl }
                },
                Scope = newScope.Name
            };
            var requestMessage = new System.Net.Http.HttpRequestMessage
            {
                RequestUri = new Uri($"{_options.SidBaseUrl}/clients"),
                Method = System.Net.Http.HttpMethod.Post,
                Content = new System.Net.Http.StringContent(JsonSerializer.Serialize(client), Encoding.UTF8, "application/json")
            };
            requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
            await httpClient.SendAsync(requestMessage);
        }
    }

    private async Task<Scope> CreateScope(HttpClient httpClient, string clientId, SamlEntrepriseMappingsResult mappings, string accessToken)
    {
        var claimMappers = new List<ScopeClaimMapper>();
        var attributes = new List<string>();
        if(mappings.DesiredAttributes != null && mappings.DesiredAttributes.Attrs != null)
        {
            if (mappings.DesiredAttributes.Attrs.RequiredUserAttributes != null) attributes.AddRange(mappings.DesiredAttributes.Attrs.RequiredUserAttributes);
            if (mappings.DesiredAttributes.Attrs.OptionalUserAttributes != null) attributes.AddRange(mappings.DesiredAttributes.Attrs.OptionalUserAttributes);
        }
        claimMappers.AddRange(attributes.Where(a => scimToSamlAttributes.ContainsKey(a)).Select(a => new ScopeClaimMapper
        {
            Id = Guid.NewGuid().ToString(),
            SAMLAttributeName = scimToSamlAttributes[a],
            SourceScimPath = a,
            MapperType = MappingRuleTypes.SCIM,
            Name = a
        }));
        claimMappers.Add(new ScopeClaimMapper
        {
            Id = Guid.NewGuid().ToString(),
            SAMLAttributeName = ClaimTypes.NameIdentifier,
            SourceScimPath = mappings.SamlSubject.Username,
            MapperType = MappingRuleTypes.SCIM,
            Name = mappings.SamlSubject.Username
        });
        var scope = new Scope
        {
            Id = Guid.NewGuid().ToString(),
            Name = $"{clientId}_saml_fastfed",
            Protocol = ScopeProtocols.SAML,
            ClaimMappers = claimMappers
        };
        var requestMessage = new System.Net.Http.HttpRequestMessage
        {
            RequestUri = new Uri($"{_options.SidBaseUrl}/scopes"),
            Method = System.Net.Http.HttpMethod.Post,
            Content = new System.Net.Http.StringContent(JsonSerializer.Serialize(scope), Encoding.UTF8, "application/json")
        };
        requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
        var httpResult = await httpClient.SendAsync(requestMessage);
        var content = await httpResult.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Scope>(content);
    }

    private async Task<string> GetAccessToken(System.Net.Http.HttpClient httpClient)
    {
        var content = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("client_id", _options.ClientId),
            new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
            new KeyValuePair<string, string>("scope", "clients scopes"),
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        };
        var httpRequest = new System.Net.Http.HttpRequestMessage
        {
            Method = System.Net.Http.HttpMethod.Post,
            RequestUri = new Uri($"{_options.SidBaseUrl}/token"),
            Content = new System.Net.Http.FormUrlEncodedContent(content)
        };
        var httpResult = await httpClient.SendAsync(httpRequest);
        var json = await httpResult.Content.ReadAsStringAsync();
        var accessToken = JsonObject.Parse(json)["access_token"].GetValue<string>();
        return accessToken;
    }
}

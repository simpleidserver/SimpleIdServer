// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.IdentityProvider.Authentication.Saml.Sid;

public class SidSamlClientProvisioningService : ISamlClientProvisioningService
{
    private readonly IdServer.Helpers.IHttpClientFactory _httpClientFactory;
    private readonly FastFedSidSamlAuthenticationOptions _options;

    public SidSamlClientProvisioningService(
        IdServer.Helpers.IHttpClientFactory httpClientFactory,
        IOptions<FastFedSidSamlAuthenticationOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    public async Task Provision(string clientId, string metadataUrl, CancellationToken cancellationToken)
    {
        using (var httpClient = _httpClientFactory.GetHttpClient())
        {
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
                }
            };
            var requestMessage = new System.Net.Http.HttpRequestMessage
            {
                RequestUri = new Uri($"{_options.SidBaseUrl}/clients"),
                Method = System.Net.Http.HttpMethod.Post,
                Content = new System.Net.Http.StringContent(JsonSerializer.Serialize(client), Encoding.UTF8, "application/json")
            };
            var accessToken = await GetAccessToken(httpClient);
            requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
            var httpResult = await httpClient.SendAsync(requestMessage);
            string ss = "";
        }
    }

    private async Task<string> GetAccessToken(System.Net.Http.HttpClient httpClient)
    {
        var content = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("client_id", _options.ClientId),
            new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
            new KeyValuePair<string, string>("scope", "clients"),
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

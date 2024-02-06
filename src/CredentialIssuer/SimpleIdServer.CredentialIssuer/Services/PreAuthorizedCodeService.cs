// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.CredentialIssuer;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.CredentialIssuer.Services;

public interface IPreAuthorizedCodeService
{
    Task<string> Get(string accessToken, List<string> scopes, CancellationToken cancellationToken);
}

public class PreAuthorizedCodeService : IPreAuthorizedCodeService
{
    private readonly Factories.IHttpClientFactory _httpClientFactory;
    private readonly CredentialIssuerOptions _options;

    public PreAuthorizedCodeService(
        Factories.IHttpClientFactory httpClientFactory,
        IOptions<CredentialIssuerOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    public async Task<string> Get(string accessToken, List<string> scopes, CancellationToken cancellationToken)
    {
        using (var httpClient = _httpClientFactory.Build())
        {
            var dic = new Dictionary<string, string>
            {
                { "client_id", _options.ClientId },
                { "client_secret", _options.ClientSecret },
                { "grant_type", "urn:ietf:params:oauth:grant-type:exchange-pre-authorized_code" },
                { "subject_token", accessToken },
                { "subject_token_type", "urn:ietf:params:oauth:token-type:access_token" },
                { "scope", string.Join((" "), scopes) }
            };
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(dic),
                RequestUri = new System.Uri($"{_options.AuthorizationServer}/token")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            httpResult.EnsureSuccessStatusCode();
            var json = await httpResult.Content.ReadAsStringAsync();
            return JsonObject.Parse(json)["pre-authorized_code"].ToString();
        }
    }
}
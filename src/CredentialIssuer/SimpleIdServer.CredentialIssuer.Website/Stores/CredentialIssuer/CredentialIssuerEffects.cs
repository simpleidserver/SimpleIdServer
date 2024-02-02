// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.CredentialIssuer.Api.CredentialIssuer;
using System.Text.Json;

namespace SimpleIdServer.CredentialIssuer.Website.Stores.CredentialIssuer;

public class CredentialIssuerEffects
{
    private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
    private readonly CredentialIssuerWebsiteOptions _options;

    public CredentialIssuerEffects(
        IWebsiteHttpClientFactory websiteHttpClientFactory,
        CredentialIssuerWebsiteOptions options)
    {
        _websiteHttpClientFactory = websiteHttpClientFactory;
        _options = options;

    }

    [EffectMethod]
    public async Task Handle(GetCredentialConfigurationsAction action, IDispatcher dispatcher)
    {
        var baseUrl = $"{_options.CredentialIssuerUrl}/.well-known/openid-credential-issuer";
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(baseUrl)
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CredentialIssuerResult>(json);
        var credentialConfigurations = result.CredentialsSupported.ToDictionary(c => c.Key, c => JsonSerializer.Deserialize<CredentialConfiguration>(c.Value.ToJsonString()));
        dispatcher.Dispatch(new GetCredentialConfigurationsSuccessAction { CredentialConfigurations = credentialConfigurations });
    }
}

public class GetCredentialConfigurationsAction { }

public class GetCredentialConfigurationsSuccessAction
{
    public Dictionary<string, CredentialConfiguration> CredentialConfigurations { get; set; }
}
// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.Extensions.Options;
using SimpleIdServer.CredentialIssuer.Domains;
using System.Text.Json;

namespace SimpleIdServer.CredentialIssuer.Website.Stores.CredentialIssuer;

public class CredentialIssuerEffects
{
    private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
    private readonly CredentialIssuerWebsiteOptions _options;

    public CredentialIssuerEffects(
        IWebsiteHttpClientFactory websiteHttpClientFactory,
        IOptions<CredentialIssuerWebsiteOptions> options)
    {
        _websiteHttpClientFactory = websiteHttpClientFactory;
        _options = options.Value;
    }

    [EffectMethod]
    public async Task Handle(GetCredentialConfigurationsAction action, IDispatcher dispatcher)
    {
        var baseUrl = GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(baseUrl)
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var credentialConfigurations = JsonSerializer.Deserialize<List<CredentialConfiguration>>(json);
        dispatcher.Dispatch(new GetCredentialConfigurationsSuccessAction { CredentialConfigurations = credentialConfigurations });
    }

    [EffectMethod]
    public async Task Handle(GetCredentialConfigurationAction action, IDispatcher dispatcher)
    {
        var baseUrl = $"{GetBaseUrl()}/{action.Id}";
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(baseUrl)
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var credentialConfiguration = JsonSerializer.Deserialize<CredentialConfiguration>(json);
        dispatcher.Dispatch(new GetCredentialConfigurationSuccessAction { Configuration = credentialConfiguration });
    }

    private string GetBaseUrl()
        => $"{_options.CredentialIssuerUrl}/credential_configurations";
}

public class GetCredentialConfigurationsAction { }

public class GetCredentialConfigurationsSuccessAction
{
    public List<CredentialConfiguration> CredentialConfigurations { get; set; }
}

public class RemoveCredentialConfigurationAction
{
    public List<string> Names { get; set; }
}

public class GetCredentialConfigurationAction
{
    public string Id { get; set; }
}

public class GetCredentialConfigurationSuccessAction
{
    public CredentialConfiguration Configuration { get; set; }
}
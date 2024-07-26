// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using SimpleIdServer.Configuration.DTOs;
using SimpleIdServer.IdServer.Website.Infrastructures;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Website.Stores.ConfigurationDefinitionStore;

public class ConfigurationDefinitionEffects
{
    private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
    private readonly IdServerWebsiteOptions _options;
    private readonly CurrentRealm _currentRealm;

    public ConfigurationDefinitionEffects(
        IWebsiteHttpClientFactory websiteHttpClientFactory, 
        IOptions<IdServerWebsiteOptions> options, 
        CurrentRealm currentRealm)
    {
        _websiteHttpClientFactory = websiteHttpClientFactory;
        _options = options.Value;
        _currentRealm = currentRealm;
    }

    [EffectMethod]
    public async Task Handle(GetAllConfigurationDefinitionsAction action, IDispatcher dispatcher)
    {
        var baseUrl = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri(baseUrl)
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var content = SidJsonSerializer.Deserialize<IEnumerable<ConfigurationDefResult>>(json);
        dispatcher.Dispatch(new GetAllConfigurationDefinitionsSuccessAction { Content = content });
    }

    private async Task<string> GetBaseUrl()
    {
        if(_options.IsReamEnabled)
        {
            var realmStr = !string.IsNullOrWhiteSpace(_currentRealm.Identifier) ? _currentRealm.Identifier : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return $"{_options.IdServerBaseUrl}/{realmStr}/confdefs";
        }

        return $"{_options.IdServerBaseUrl}/confdefs";
    }
}

public class GetAllConfigurationDefinitionsAction { }

public class GetAllConfigurationDefinitionsSuccessAction
{
    public IEnumerable<ConfigurationDefResult> Content { get; set; }
}
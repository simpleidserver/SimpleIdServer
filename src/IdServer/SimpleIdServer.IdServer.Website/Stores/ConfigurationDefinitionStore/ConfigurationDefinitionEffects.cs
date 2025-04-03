// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.ConfDefs;
using SimpleIdServer.IdServer.Helpers;

namespace SimpleIdServer.IdServer.Website.Stores.ConfigurationDefinitionStore;

public class ConfigurationDefinitionEffects
{
    private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
    private readonly IdServerWebsiteOptions _options;
    private readonly IRealmStore _realmStore;

    public ConfigurationDefinitionEffects(
        IWebsiteHttpClientFactory websiteHttpClientFactory, 
        IOptions<IdServerWebsiteOptions> options,
        IRealmStore realmStore)
    {
        _websiteHttpClientFactory = websiteHttpClientFactory;
        _options = options.Value;
        _realmStore = realmStore;
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
            var realm = _realmStore.Realm;
            var realmStr = !string.IsNullOrWhiteSpace(realm) ? realm : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return $"{_options.Issuer}/{realmStr}/confdefs";
        }

        return $"{_options.Issuer}/confdefs";
    }
}

public class GetAllConfigurationDefinitionsAction { }

public class GetAllConfigurationDefinitionsSuccessAction
{
    public IEnumerable<ConfigurationDefResult> Content { get; set; }
}
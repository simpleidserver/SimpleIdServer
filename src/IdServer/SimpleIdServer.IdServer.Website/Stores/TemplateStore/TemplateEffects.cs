// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using FormBuilder.Models;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Helpers;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Website.Stores.TemplateStore;

public class TemplateEffects
{
    private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
    private readonly IdServerWebsiteOptions _options;
    private readonly IRealmStore _realmStore;

    public TemplateEffects(IWebsiteHttpClientFactory websiteHttpClientFactory, IOptions<IdServerWebsiteOptions> options, IRealmStore realmStore)
    {
        _websiteHttpClientFactory = websiteHttpClientFactory;
        _options = options.Value;
        _realmStore = realmStore;
    }

    [EffectMethod]
    public async Task Handle(GetActiveTemplateAction action, IDispatcher dispatcher)
    {
        var baseUrl = await GetTemplatesUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{baseUrl}/active"),
            Method = HttpMethod.Get
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var template = JsonSerializer.Deserialize<Template>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        dispatcher.Dispatch(new GetActiveTemplateSuccessAction { Template = template });
    }

    private async Task<string> GetTemplatesUrl()
    {
        if (_options.IsReamEnabled)
        {
            var realm = _realmStore.Realm;
            var realmStr = !string.IsNullOrWhiteSpace(realm) ? realm : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return $"{_options.Issuer}/{realmStr}/templates";
        }

        return $"{_options.Issuer}/templates";
    }
}

public class GetActiveTemplateAction
{

}

public class GetActiveTemplateSuccessAction
{
    public Template Template { get; set; }
}

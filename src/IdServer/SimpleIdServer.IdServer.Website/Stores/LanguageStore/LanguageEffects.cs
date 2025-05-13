// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.LanguageStore;

public class LanguageEffects
{
    private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
    private readonly IdServerWebsiteOptions _options;

    public LanguageEffects(
        IWebsiteHttpClientFactory websiteHttpClientFactory, 
        IOptions<IdServerWebsiteOptions> options)
    {
        _websiteHttpClientFactory = websiteHttpClientFactory;
        _options = options.Value;
    }

    [EffectMethod]
    public async Task Handle(GetLanguagesAction action, IDispatcher dispatcher)
    {
        var url = GetLanguagesUrl();
        var httpClient = await _websiteHttpClientFactory.Build(action.Realm);
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Get
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var languages = SidJsonSerializer.Deserialize<List<Language>>(json);
        dispatcher.Dispatch(new GetLanguagesSuccessAction { Languages = languages });
    }

    private string GetLanguagesUrl() => $"{_options.Issuer}/languages";
}

public class GetLanguagesAction
{
    public string Realm
    {
        get; set;
    }
}

public class GetLanguagesSuccessAction
{
    public List<Language> Languages { get; set; }
}
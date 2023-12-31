// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Website.Stores.LanguageStore;

public class LanguageEffects
{
    private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
    private readonly IdServerWebsiteOptions _options;
    private readonly ProtectedSessionStorage _sessionStorage;

    public LanguageEffects(IWebsiteHttpClientFactory websiteHttpClientFactory, IOptions<IdServerWebsiteOptions> options, ProtectedSessionStorage sessionStorage)
    {
        _websiteHttpClientFactory = websiteHttpClientFactory;
        _options = options.Value;
        _sessionStorage = sessionStorage;
    }


    [EffectMethod]
    public async Task Handle(GetLanguagesAction action, IDispatcher dispatcher)
    {
        var url = GetLanguagesUrl();
        var defaultLanguage = await GetDefaultLanguage();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Get
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var languages = JsonSerializer.Deserialize<List<Language>>(json);
        dispatcher.Dispatch(new GetLanguagesSuccessAction { Languages = languages });
        dispatcher.Dispatch(new SetDefaultLanguageSuccessAction { Language = defaultLanguage });
    }

    [EffectMethod]
    public async Task Handle(SetDefaultLanguageAction action, IDispatcher dispatcher)
    {
        await _sessionStorage.SetAsync("language", action.Language);
        dispatcher.Dispatch(new SetDefaultLanguageSuccessAction { Language = action.Language });
    }

    private string GetLanguagesUrl() => $"{_options.IdServerBaseUrl}/languages";

    private async Task<string> GetDefaultLanguage()
    {
        var language = (await _sessionStorage.GetAsync<string>("language")).Value;
        return language ?? Language.Default;
    }
}

public class GetLanguagesAction
{
    
}

public class GetLanguagesSuccessAction
{
    public List<Language> Languages { get; set; }
}

public class SetDefaultLanguageAction
{
    public string Language { get; set; }
}

public class SetDefaultLanguageSuccessAction
{
    public string Language { get; set; }
}
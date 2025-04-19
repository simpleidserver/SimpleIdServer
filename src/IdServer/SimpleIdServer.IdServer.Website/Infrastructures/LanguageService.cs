// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Infrastructures;

public interface ILanguageService
{
    Task<List<string>> GetSupportedLanguagesAsync();
}

public class LanguageService : ILanguageService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IdServerWebsiteOptions _options;

    public LanguageService(
        IHttpClientFactory httpClientFactory, 
        IOptions<IdServerWebsiteOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    public async Task<List<string>> GetSupportedLanguagesAsync()
    {
        var realm = _options.IsReamEnabled ? Constants.DefaultRealm : null;
        using (var httpClient = _httpClientFactory.CreateClient())
        {
            var url = $"{_options.Issuer}/languages";
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            if (!httpResult.IsSuccessStatusCode)
            {
                return new List<string> { Constants.DefaultLanguage };
            }

            var json = await httpResult.Content.ReadAsStringAsync();
            var languages = SidJsonSerializer.Deserialize<List<Language>>(json);
            var languageCodes = languages.Select(l => l.Code).ToList();
            return languageCodes;
        }
    }
}

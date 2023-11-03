// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.AuthenticationMethods;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Website.Stores.AuthMethodsStore;

public class AuthMethodEffects
{
    private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
    private readonly IdServerWebsiteOptions _options;
    private readonly ProtectedSessionStorage _sessionStorage;

    public AuthMethodEffects(IWebsiteHttpClientFactory websiteHttpClientFactory, IOptions<IdServerWebsiteOptions> options, ProtectedSessionStorage sessionStorage)
    {
        _websiteHttpClientFactory = websiteHttpClientFactory;
        _options = options.Value;
        _sessionStorage = sessionStorage;
    }

    [EffectMethod]
    public async Task Handle(GetAllAuthMethodAction action, IDispatcher dispatcher)
	{
        var baseUrl = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(baseUrl)
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<IEnumerable<AuthenticationMethodResult>>(json);
        dispatcher.Dispatch(new GetAllAuthMethodSuccessAction { AuthMethods = result });
    }

    [EffectMethod]
    public async Task Handle(GetAuthMethodAction action, IDispatcher dispatcher)
    {
        var baseUrl = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{baseUrl}/{action.Amr}")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        try
        {
            httpResult.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<AuthenticationMethodResult>(json);
            dispatcher.Dispatch(new GetAuthMethodSuccessAction { AuthMethod = result });
        }
        catch
        {
            var jsonObj = JsonObject.Parse(json);
            dispatcher.Dispatch(new GetAuthMethodFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
        }
    }

    [EffectMethod]
    public async Task Handle(UpdateAuthMethodAction action, IDispatcher dispatcher)
    {
        var baseUrl = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestContent = new UpdateAuthMethodConfigurationsRequest
        {
            Values = action.Values
        };
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri($"{baseUrl}/{action.Amr}"),
            Content = new StringContent(JsonSerializer.Serialize(requestContent), System.Text.Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        try
        {
            httpResult.EnsureSuccessStatusCode();
            dispatcher.Dispatch(new UpdateAuthMethodSuccessAction { Amr = action.Amr, Values = action.Values });
        }
        catch
        {
            var jsonObj = JsonObject.Parse(json);
            dispatcher.Dispatch(new UpdateAuthMethodFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
        }
    }

    private async Task<string> GetBaseUrl()
    {
        if(_options.IsReamEnabled)
        {
            var realm = await _sessionStorage.GetAsync<string>("realm");
            var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return $"{_options.IdServerBaseUrl}/{realmStr}/authmethods";
        }

        return $"{_options.IdServerBaseUrl}/authmethods";
    }
}

public class GetAllAuthMethodAction
{

}

public class GetAllAuthMethodSuccessAction
{
    public IEnumerable<AuthenticationMethodResult> AuthMethods { get; set; }
}

public class GetAuthMethodAction
{
    public string Amr { get; set; }
}

public class GetAuthMethodSuccessAction
{
    public AuthenticationMethodResult AuthMethod { get; set; }
}

public class GetAuthMethodFailureAction
{
    public string ErrorMessage { get; set; }
}

public class UpdateAuthMethodAction
{
    public string Amr { get; set; }
    public Dictionary<string, string> Values { get; set; }
}

public class UpdateAuthMethodSuccessAction
{
    public string Amr { get; set; }
    public Dictionary<string, string> Values { get; set; }
}

public class UpdateAuthMethodFailureAction
{
    public string ErrorMessage { get; set; }
}
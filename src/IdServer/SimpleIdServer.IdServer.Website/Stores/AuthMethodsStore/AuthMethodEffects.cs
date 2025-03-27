// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.AuthenticationMethods;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Website.Infrastructures;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Website.Stores.AuthMethodsStore;

public class AuthMethodEffects
{
    private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
    private readonly IdServerWebsiteOptions _options;
    private readonly IRealmStore _realmStore;

    public AuthMethodEffects(
        IWebsiteHttpClientFactory websiteHttpClientFactory, 
        IOptions<IdServerWebsiteOptions> options,
        IRealmStore realmStore)
    {
        _websiteHttpClientFactory = websiteHttpClientFactory;
        _options = options.Value;
        _realmStore = realmStore;
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
        var result = SidJsonSerializer.Deserialize<IEnumerable<AuthenticationMethodResult>>(json);
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
            var result = SidJsonSerializer.Deserialize<AuthenticationMethodResult>(json);
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

    [EffectMethod]
    public async Task Handle(GetUserLockingOptionsAction action, IDispatcher dispatcher)
    {
        var baseUrl = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{baseUrl}/userlockingoptions")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<UserLockingResult>(json);
        dispatcher.Dispatch(new GetUserLockingOptionsSuccessAction { UserLocking = result });
    }

    [EffectMethod]
    public async Task Handle(UpdateUserLockingOptionsAction action, IDispatcher dispatcher)
    {
        var baseUrl = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri($"{baseUrl}/userlockingoptions"),
            Content = new StringContent(JsonSerializer.Serialize(new UpdateUserLockingRequest
            {
                Values = action.Values
            }), Encoding.UTF8, "application/json")
        };
        await httpClient.SendAsync(requestMessage);
        dispatcher.Dispatch(new UpdateUserLockingOptionsSuccessAction());
    }

    private async Task<string> GetBaseUrl()
    {
        if(_options.IsReamEnabled)
        {
            var realm = RealmContext.Instance()?.Realm;
            var realmStr = !string.IsNullOrWhiteSpace(realm) ? realm : SimpleIdServer.IdServer.Constants.DefaultRealm;
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

public class StartAddAcrMethodAction
{

}

public class GetUserLockingOptionsAction
{

}

public class GetUserLockingOptionsSuccessAction
{
    public UserLockingResult UserLocking { get; set; }
}

public class UpdateUserLockingOptionsAction
{
    public Dictionary<string, string> Values { get; set; }
}

public class UpdateUserLockingOptionsSuccessAction
{

}
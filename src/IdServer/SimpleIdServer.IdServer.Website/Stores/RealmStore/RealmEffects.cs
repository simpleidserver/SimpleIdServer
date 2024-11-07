// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Realms;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Website.Resources;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Website.Stores.RealmStore;

public class RealmEffects
{
    private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
    private readonly IdServerWebsiteOptions _options;

    public RealmEffects(IWebsiteHttpClientFactory websiteHttpClientFactory, IOptions<IdServerWebsiteOptions> options)
    {
        _websiteHttpClientFactory = websiteHttpClientFactory;
        _options = options.Value;
    }


    [EffectMethod]
    public async Task Handle(GetAllRealmAction action, IDispatcher dispatcher)
    {
        var url = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Get
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var realms = SidJsonSerializer.Deserialize<IEnumerable<Realm>>(json);
        dispatcher.Dispatch(new GetAllRealmSuccessAction { Realms = realms });
    }

    [EffectMethod]
    public async Task Handle(AddRealmAction action, IDispatcher dispatcher)
    {
        if(!_options.IsReamEnabled)
        {
            dispatcher.Dispatch(new AddRealmFailureAction { ErrorMessage = Global.CannotAddRealmBecauseOptionIsDisabled });
            return;
        }

        var url = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var req = new AddRealmRequest
        {
            Name = action.Name,
            Description = action.Description
        };
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        try
        {
            httpResult.EnsureSuccessStatusCode();
            dispatcher.Dispatch(new AddRealmSuccessAction
            {
                Description = action.Description,
                Name = action.Name
            });
        }
        catch
        {
            var jsonObj = JsonObject.Parse(json);
            dispatcher.Dispatch(new AddRealmFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
        }
    }

    [EffectMethod]
    public async Task Handle(DeleteCurrentRealmAction action, IDispatcher dispatcher)
    {
        var url = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var realm = RealmContext.Instance()?.Realm;
        var realmStr = !string.IsNullOrWhiteSpace(realm) ? realm : SimpleIdServer.IdServer.Constants.DefaultRealm;
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{url}/{realmStr}"),
            Method = HttpMethod.Delete
        };
        await httpClient.SendAsync(requestMessage);
        _websiteHttpClientFactory.RemoveAccessToken(realmStr);
        dispatcher.Dispatch(new DeleteCurrentRealmSuccessAction());
    }

    private async Task<string> GetBaseUrl()
    {
        if (_options.IsReamEnabled)
        {
            var realm = RealmContext.Instance()?.Realm;
            var realmStr = !string.IsNullOrWhiteSpace(realm) ? realm : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return $"{_options.IdServerBaseUrl}/{realmStr}/realms";
        }

        return $"{_options.IdServerBaseUrl}/realms";
    }
}

public class GetAllRealmAction
{
    public IEnumerable<Domains.Realm> Realms { get; set; }
}

public class GetAllRealmSuccessAction
{
    public IEnumerable<Domains.Realm> Realms { get; set; }
}

public class AddRealmAction
{
    public string Name { get; set; }
    public string Description { get; set; }
}

public class AddRealmFailureAction
{
    public string ErrorMessage { get; set; }
}

public class AddRealmSuccessAction
{
    public string Name { get; set; }
    public string Description { get; set; }
}

public class DeleteCurrentRealmAction
{
}

public class DeleteCurrentRealmSuccessAction
{
}
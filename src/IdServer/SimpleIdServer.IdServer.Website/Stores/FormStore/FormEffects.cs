﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using FormBuilder.Models;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Website.Stores.ClientStore;
using System.Text;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Website.Stores.FormStore;

public class FormEffects
{
    private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
    private readonly IdServerWebsiteOptions _configuration;

    public FormEffects(
        IWebsiteHttpClientFactory websiteHttpClientFactory,
        IOptions<IdServerWebsiteOptions> configuration)
    {
        _websiteHttpClientFactory = websiteHttpClientFactory;
        _configuration = configuration.Value;
    }


    [EffectMethod]
    public async Task Handle(GetFormAction action, IDispatcher dispatcher)
    {
        var baseUrl = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{baseUrl}/{action.Id}"),
            Method = HttpMethod.Get
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var settings = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var formRecord = JsonSerializer.Deserialize<FormRecord>(json, settings);
        dispatcher.Dispatch(new GetFormSuccessAction { Form = formRecord });
    }

    [EffectMethod]
    public async Task Handle(UpdateFormAction action, IDispatcher dispatcher)
    {
        var baseUrl = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{baseUrl}/{action.Id}"),
            Method = HttpMethod.Put,
            Content = new StringContent(JsonSerializer.Serialize(action.Form), Encoding.UTF8, "application/json")
        };
        await httpClient.SendAsync(requestMessage);
        dispatcher.Dispatch(new UpdateFormSuccessAction());
    }

    [EffectMethod]
    public async Task Handle(PublishFormAction action, IDispatcher dispatcher)
    {
        var baseUrl = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{baseUrl}/{action.Id}/publish"),
            Method = HttpMethod.Post
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var settings = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var form = JsonSerializer.Deserialize<FormRecord>(json, settings);
        dispatcher.Dispatch(new PublishFormSuccessAction
        {
            Form = form
        });
    }

    private async Task<string> GetBaseUrl()
    {
        if (_configuration.IsReamEnabled)
        {
            var realm = RealmContext.Instance()?.Realm;
            var realmStr = !string.IsNullOrWhiteSpace(realm) ? realm : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return $"{_configuration.IdServerBaseUrl}/{realmStr}/forms";
        }

        return $"{_configuration.IdServerBaseUrl}/forms";
    }
}

public class GetFormAction
{
    public string Id { get; set; }
}

public class GetFormSuccessAction
{
    public FormRecord Form { get; set; }
}

public class UpdateFormAction
{
    public string Id { get; set; }
    public FormRecord Form { get; set; }
}

public class UpdateFormSuccessAction
{

}

public class PublishFormAction
{
    public string Id { get; set; }
}

public class PublishFormSuccessAction
{
    public FormRecord Form { get; set; }
}
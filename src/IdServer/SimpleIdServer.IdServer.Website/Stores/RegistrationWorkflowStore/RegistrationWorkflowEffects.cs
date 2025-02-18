// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.RegistrationWorkflows;
using SimpleIdServer.IdServer.Helpers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Website.Stores.RegistrationWorkflowStore;

public class RegistrationWorkflowEffects
{
    private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
    private readonly IdServerWebsiteOptions _options;
    private readonly IRealmStore _realmStore;

    public RegistrationWorkflowEffects(
        IWebsiteHttpClientFactory websiteHttpClientFactory, 
        IOptions<IdServerWebsiteOptions> options,
        IRealmStore realmStore)
    {
        _websiteHttpClientFactory = websiteHttpClientFactory;
        _options = options.Value;
        _realmStore = realmStore;
    }

    [EffectMethod]
    public async Task Handle(GetAllRegistrationWorkflowsAction action, IDispatcher dispatcher)
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
        var result = SidJsonSerializer.Deserialize<List<RegistrationWorkflowResult>>(json);
        dispatcher.Dispatch(new GetAllRegistrationWorkflowsSuccessAction { RegistrationWorkflows = result });
    }

    [EffectMethod]
    public async Task Handle(GetRegistrationWorkflowAction action, IDispatcher dispatcher)
    {
        var baseUrl = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{baseUrl}/{action.Id}")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        try
        {
            httpResult.EnsureSuccessStatusCode();
            var result = SidJsonSerializer.Deserialize<RegistrationWorkflowResult>(json);
            dispatcher.Dispatch(new GetRegistrationWorkflowSuccessAction { RegistrationWorkflow = result });
        }
        catch
        {
            var jsonObj = JsonObject.Parse(json);
            dispatcher.Dispatch(new GetRegistrationWorkflowFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
        }
    }

    [EffectMethod]
    public async Task Handle(RemoveSelectedRegistrationWorkflowsAction action, IDispatcher dispatcher)
    {
        var baseUrl = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        foreach(var id in action.Ids)
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{baseUrl}/{id}")
            };
            await httpClient.SendAsync(requestMessage);
        }

        dispatcher.Dispatch(new RemoveSelectedRegistrationWorkflowsSuccessAction { Ids = action.Ids });
    }

    [EffectMethod]
    public async Task Handle(UpdateRegistrationWorkflowAction action, IDispatcher dispatcher)
    {
        var baseUrl = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var jsonRequest = JsonSerializer.Serialize(new RegistrationWorkflowResult
        {
            Name = action.Name,
            IsDefault = action.IsDefault
        });
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{baseUrl}/{action.Id}"),
            Method = HttpMethod.Put,
            Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        try
        {
            httpResult.EnsureSuccessStatusCode();
            dispatcher.Dispatch(new UpdateRegistrationWorkflowSuccessAction { Id = action.Id, IsDefault = action.IsDefault, Name = action.Name  });
        }
        catch
        {
            var json = await httpResult.Content.ReadAsStringAsync();
            var jsonObj = JsonObject.Parse(json);
            dispatcher.Dispatch(new UpdateRegistrationWorkflowFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
        }
    }

    [EffectMethod]
    public async Task Handle(AddRegistrationWorkflowAction action, IDispatcher dispatcher)
    {
        var baseUrl = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var jsonRequest = JsonSerializer.Serialize(new RegistrationWorkflowResult
        {
            Name = action.Name,
            IsDefault = action.IsDefault
        });
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri(baseUrl),
            Method = HttpMethod.Post,
            Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        try
        {
            httpResult.EnsureSuccessStatusCode();
            var result = SidJsonSerializer.Deserialize<RegistrationWorkflowResult>(json);
            dispatcher.Dispatch(new AddRegistrationWorkflowSuccessAction { Id = result.Id, IsDefault = result.IsDefault, Name = result.Name });
        }
        catch
        {
            var jsonObj = JsonObject.Parse(json);
            dispatcher.Dispatch(new AddRegistrationWorkflowFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
        }
    }

    private async Task<string> GetBaseUrl()
    {
        if(_options.IsReamEnabled)
        {
            var realm = _realmStore.Realm;
            var realmStr = !string.IsNullOrWhiteSpace(realm) ? realm : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return $"{_options.IdServerBaseUrl}/{realmStr}/registrationworkflows";
        }

        return $"{_options.IdServerBaseUrl}/registrationworkflows";
    }
}

public class GetAllRegistrationWorkflowsAction
{

}

public class GetAllRegistrationWorkflowsSuccessAction
{
    public List<RegistrationWorkflowResult> RegistrationWorkflows { get; set; }
}

public class GetRegistrationWorkflowAction
{
    public string Id { get; set; }
}

public class GetRegistrationWorkflowSuccessAction
{
    public RegistrationWorkflowResult RegistrationWorkflow { get; set; }
}

public class GetRegistrationWorkflowFailureAction
{
    public string ErrorMessage { get; set; }
}

public class RemoveSelectedRegistrationWorkflowsAction
{
    public List<string> Ids { get; set; }
}

public class RemoveSelectedRegistrationWorkflowsSuccessAction
{
    public List<string> Ids { get; set; }
}

public class UpdateRegistrationWorkflowAction
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsDefault { get; set; }
}

public class UpdateRegistrationWorkflowSuccessAction
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsDefault { get; set; }
}

public class UpdateRegistrationWorkflowFailureAction
{
    public string ErrorMessage { get; set; }
}

public class AddRegistrationWorkflowAction
{
    public string Name { get; set; }
    public bool IsDefault { get; set; }
}

public class AddRegistrationWorkflowSuccessAction
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsDefault { get; set; }
}

public class AddRegistrationWorkflowFailureAction
{
    public string ErrorMessage { get; set; }
}

public class ToggleRegistrationWorkflowAction
{
    public string Id { get; set; }
    public bool IsSelected { get; set; }
}

public class ToggleAllRegistrationWorkflowAction
{
    public bool IsSelected { get; set; }
}
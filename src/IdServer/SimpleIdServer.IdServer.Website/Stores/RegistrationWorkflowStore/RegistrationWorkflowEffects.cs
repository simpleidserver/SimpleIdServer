// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.RegistrationWorkflows;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Website.Stores.RegistrationWorkflowStore;

public class RegistrationWorkflowEffects
{
    private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
    private readonly IdServerWebsiteOptions _options;
    private readonly ProtectedSessionStorage _sessionStorage;

    public RegistrationWorkflowEffects(IWebsiteHttpClientFactory websiteHttpClientFactory, IOptions<IdServerWebsiteOptions> options, ProtectedSessionStorage sessionStorage)
    {
        _websiteHttpClientFactory = websiteHttpClientFactory;
        _options = options.Value;
        _sessionStorage = sessionStorage;
    }

    [EffectMethod]
    public async Task Handle(GetAllRegistrationWorkflowsAction action, IDispatcher dispatcher)
    {
        var realm = await GetRealm();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/registrationworkflows")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<List<RegistrationWorkflowResult>>(json);
        dispatcher.Dispatch(new GetAllRegistrationWorkflowsSuccessAction { RegistrationWorkflows = result });
    }

    [EffectMethod]
    public async Task Handle(GetRegistrationWorkflowAction action, IDispatcher dispatcher)
    {
        var realm = await GetRealm();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/registrationworkflows/{action.Id}")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        try
        {
            httpResult.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<RegistrationWorkflowResult>(json);
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
        var realm = await GetRealm();
        var httpClient = await _websiteHttpClientFactory.Build();
        foreach(var id in action.Ids)
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/registrationworkflows/{id}")
            };
            await httpClient.SendAsync(requestMessage);
        }

        dispatcher.Dispatch(new RemoveSelectedRegistrationWorkflowsSuccessAction { Ids = action.Ids });
    }

    [EffectMethod]
    public async Task Handle(UpdateRegistrationWorkflowAction action, IDispatcher dispatcher)
    {
        var realm = await GetRealm();
        var httpClient = await _websiteHttpClientFactory.Build();
        var jsonRequest = JsonSerializer.Serialize(new RegistrationWorkflowResult
        {
            Name = action.Name,
            IsDefault = action.IsDefault,
            Steps = action.Steps
        });
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/registrationworkflows/{action.Id}"),
            Method = HttpMethod.Put,
            Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        try
        {
            httpResult.EnsureSuccessStatusCode();
            dispatcher.Dispatch(new UpdateRegistrationWorkflowSuccessAction { Id = action.Id, IsDefault = action.IsDefault, Name = action.Name, Steps = action.Steps  });
        }
        catch
        {
            var json = await httpResult.Content.ReadAsStringAsync();
            var jsonObj = JsonObject.Parse(json);
            dispatcher.Dispatch(new UpdateRegistrationWorkflowFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
        }
    }

    private async Task<string> GetRealm()
    {
        var realm = await _sessionStorage.GetAsync<string>("realm");
        var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
        return realmStr;
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
    public List<string> Steps { get; set; }
}

public class UpdateRegistrationWorkflowSuccessAction
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsDefault { get; set; }
    public List<string> Steps { get; set; }
}

public class UpdateRegistrationWorkflowFailureAction
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